using System.Text;
using System.Text.RegularExpressions;

namespace SplitBill.Data.Compat;

internal class Receipt_v1_0_Deserializer(StorageFile location) : ReceiptForm(location), IReceiptCompatDeserializer
{
    public static Version Version => new(1, 0);

    public override async Task<ReceiptForm> LoadAsync()
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
                if (line is null || line[0] == '═')
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
}
