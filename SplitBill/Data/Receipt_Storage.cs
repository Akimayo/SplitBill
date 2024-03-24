using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using SplitBill.Data.Compat;
using SplitBill.Models;

namespace SplitBill.Data;
internal partial class Receipt
{
    /// <summary>
    /// Version of receipt format for deserialization
    /// </summary>
    protected static readonly Version FormatVersion = new(1, 1);
    protected static readonly Regex MetaLine = MetaLineRegex();
    protected StorageFile _location;
    protected long _streamMetaEnd = 0;
    private Type _formType;
    private async Task LoadMetadataAsync()
    {
        try
        {
            Stream s = await this._location.OpenStreamForReadAsync();
            string[] lines = { "", "", "", "", "" };
            using (StreamReader reader = new(s))
            {
                int index = 0;
                string? line;
                while (index < lines.Length)
                {
                    line = await reader.ReadLineAsync();
                    if (line == null) return; // Fail if file ends early
                    if (line[line.Length - 1] == '~')
                    {
                        lines[index] += line.Substring(0, line.Length - 1);
                    }
                    else
                    {
                        lines[index] += line;
                        index++;
                    }
                }
                this._streamMetaEnd = reader.ActualPosition();
            }
            if (!MetaLine.IsMatch(lines[3])) return; // Fail gracefuly when metadata is incorrect
            MatchCollection metadata = MetaLine.Matches(lines[3]);
            Version receiptVersion = Version.Parse(metadata[0].Groups[1].Value);
            if (receiptVersion.Equals(FormatVersion))
                this._formType = typeof(ReceiptForm);
            else
            {
                IEnumerable<Type> q = from t in Assembly.GetExecutingAssembly().GetTypes()
                                      where t.IsClass && t.Namespace == "SplitBill.Data.Compat" && !t.IsNested
                                      select t;
                if (!q.Any()) return; // Fail gracefuly when no compat deserializers were found
                Type compat = q.First(t => t.GetProperty(nameof(IReceiptCompatDeserializer.Version), BindingFlags.Public | BindingFlags.Static).GetValue(null).Equals(receiptVersion));
                if (compat == null) return; // Fail gracefuly when version is other than one of supported
                this._formType = compat;
            }
            this.Currency = metadata[0].Groups[3].Value;
            this.Title = lines[0];
            this.Date = DateTime.ParseExact(lines[1], "R", CultureInfo.InvariantCulture);
            this.IsValid = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    public Task<ReceiptForm> LoadAsync()
    {
        ReceiptForm form = Activator.CreateInstance(this._formType, this._location) as ReceiptForm;
        form.Currency = this.Currency;
        form.Date = this.Date;
        form.IsValid = this.IsValid;
        form.Title = this.Title;
        form._streamMetaEnd = this._streamMetaEnd;
        return form.LoadAsync();
    }

    [GeneratedRegex(@"SplitBill\s+V\[((\d+\.)+\d+)\]\s+C\[(.*?)\]")]
    private static partial Regex MetaLineRegex();
}
