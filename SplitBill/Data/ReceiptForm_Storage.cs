using System.Text;
using System.Text.RegularExpressions;

namespace SplitBill.Data;

internal partial class ReceiptForm
{
    protected static readonly Regex ItemFormat = ItemFormatRegex();
    protected static readonly Regex ParticipantFormat = ParticipantFormatRegex();
    public new virtual async Task<ReceiptForm> LoadAsync()
    {
        if (!this._initialized) await this.WriteAsync();
        this._initialized = true;
        Stream s = await this._location.OpenStreamForReadAsync();
        s.Seek(this._streamMetaEnd, SeekOrigin.Begin);
        using (StreamReader reader = new(s))
        {
            string? line;
            StringBuilder fullLine = new();
            ReceiptItem? lastItem = null;
            int p;
            byte amt;
            string name;
            while (true)
            {
                line = await reader.ReadLineAsync();
                if (line is null || line[0] == '-')
                {
                    if (!string.IsNullOrEmpty(line) && line.Length >= LINE_WIDTH) this.LineWidth = line.Length;
                    break; // End reading when file ends or separator is encountered
                }
                if (ParticipantFormat.IsMatch(line) && lastItem != null)
                { // When the line matches the participant format, add the participant
                    fullLine.Append(line);
                    GroupCollection matches = ParticipantFormat.Matches(fullLine.ToString())[0].Groups;
                    name = matches[2].Value.Trim();
                    if (!this.Names.Contains(name)) this.Names.Add(name);
                    p = this.Names.IndexOf(name);
                    amt = string.IsNullOrEmpty(matches[4].Value) ? (byte)1 : byte.Parse(matches[4].Value);
                    if (p > lastItem.Participants.Count) lastItem.Participants.AddRange(new byte[p]);
                    lastItem.Participants.Insert(p, amt);
                    fullLine.Clear();
                }
                else if (ItemFormat.IsMatch(line))
                {
                    // When the line matches the item format, add the item
                    fullLine.Append(line);
                    GroupCollection matches = ItemFormat.Matches(fullLine.ToString())[0].Groups;
                    lastItem = new ReceiptItem(this, matches[1].Value.Trim(), (int)(float.Parse(matches[3].Value) * 100));
                    this.Items.Add(lastItem);
                    fullLine.Clear();
                }
                else if (line[^1] == '~') // Continue separated line
                    fullLine.Append(line.AsSpan(0, line.Length - 1));
                else
                { // Reset line and last item on invalid lines
                    fullLine.Clear();
                    lastItem = null;
                }
            }
        }
        this.IsSaving = false;
        return this;
    }
    protected async Task WriteAsync()
    {
        Stream s = await this._location.OpenStreamForWriteAsync();
        using (StreamWriter writer = new(s, Encoding.ASCII))
            await writer.WriteAsync(this.ToString());
    }

    private async void SaveReceipt(object? sender, object e)
    {
        this._timer.Stop();
        await this.WriteAsync();
        this.IsSaving = false;
        this.OnPropertyChanged(nameof(this.TextReceipt));
    }

    [GeneratedRegex(@"^(.+)?\s+([^\d]+)(\-?\d+[,\.]\d{2})\s*$")]
    private static partial Regex ItemFormatRegex();
    [GeneratedRegex(@"^(\||\s)\s(.+?)(\s×(\d+))?\s*$")]
    private static partial Regex ParticipantFormatRegex();
}
