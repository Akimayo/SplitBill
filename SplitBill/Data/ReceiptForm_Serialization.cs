using System.Text;

namespace SplitBill.Data;
internal partial class ReceiptForm
{
    /// <summary>
    /// Maximum width of line
    /// </summary>
    protected const int LINE_WIDTH = 30; // Usually one of: 30, 40, 42, 56
    private int _lineWidth = LINE_WIDTH;
    public int LineWidth { get => this._lineWidth; set { this._lineWidth = value; this.OnPropertyChanged(); } }
    /// <summary>
    /// Width
    /// </summary>
    private const int PRICE_CHARS = 6 + 3;

    public override string ToString()
    {
        int lw3 = this.LineWidth / 3;

        StringBuilder sb = new();
        // Header with metadata
        sb.AppendLine(Limit(this.Title))                                                                                                // <Receipt Title>
          .AppendLine(this.Date.ToString("R"))                                                                                          // <Date>
          .AppendLine("".PadLeft(this.LineWidth, '*'))                                                                                  // ***********************
          .Append("SplitBill".PadRight(lw3)).Append($"V[{FormatVersion.ToString(2)}]".PadRight(lw3)).AppendLine($"C[{this.Currency}]")  // SplitBill  V[1.0]  C[$]
          .AppendLine("".PadRight(this.LineWidth, '*'));                                                                                // ***********************
        // Items list
        foreach (ReceiptItem i in this.Items)
        {
            sb.Append(LimitEntry(i.Name, 0)).AppendLine(FormatPrice(i.Price));                                                          // <Item>           <0.00>
            for (int p = 0; p < this.Names.Count; p++)
            {
                if (i.Participants.Count > p)
                {
                    if (i.Participants[p] > 1) sb.AppendLine(LimitEntry("| " + this.Names[p] + " ×" + i.Participants[p], 2).TrimEnd()); // | <Name> ×2
                    else if (i.Participants[p] > 0) sb.AppendLine(LimitEntry("| " + this.Names[p], 2).TrimEnd());                       // | <Name>
                }
            }
        }
        int totalPrice = this.TotalPrice;
        sb.AppendLine("".PadRight(this.LineWidth, '-'))                                                                                 // -----------------------
          .Append(LimitEntry("TOTAL", 0)).AppendLine(FormatPrice(totalPrice));                                                          // TOTAL            <0.00>
        int subtotal = 0, priceFor;
        for (int p = 0; p < this.Names.Count; p++)
        {
            priceFor = this.Items.Sum(i => i.PriceFor(p));
            sb.Append(LimitEntry("| " + this.Names[p], 2)).AppendLine(FormatPrice(priceFor));                                           // | <Name>         <0.00>
            subtotal += priceFor;
        }
        sb.Append(LimitEntry("└ (rest)", 2)).AppendLine(FormatPrice(totalPrice - subtotal));                                            // └ (rest)         <0.00>
        return sb.ToString();
    }

    /// <summary>
    /// Limits the width of <paramref name="text"/> in a controlled way.
    /// </summary>
    /// <param name="text">Text to be potentially split</param>
    /// <returns>The given <paramref name="text"/> with a maximum width of <see cref="LINE_WIDTH"/></returns>
    public string Limit(string text)
    {
        int len = text.Length;
        if (len <= this.LineWidth) return text;
        StringBuilder sb = new();
        for (int i = 0; i < len / this.LineWidth; i++)
            sb.Append(string.Concat(text.AsSpan(i * this.LineWidth, this.LineWidth - 1), "~\n"));
        sb.Remove(sb.Length - 2, 2);
        return sb.ToString();
    }
    /// <summary>
    /// Limits the width of <paramref name="text"/> in a controlled way, also accounting for <see cref="Currency"/> and given <paramref name="offset"/>.
    /// </summary>
    /// <param name="text">Text to be potentially split</param>
    /// <param name="offset">Number of spaces to prepend to new lines</param>
    /// <returns>The given <paramref name="text"/> with a maximum width of <see cref="LINE_WIDTH"/> also accounting for <see cref="Currency"/> and <paramref name="offset"/></returns>
    public string LimitEntry(string text, byte offset)
    {
        int maxWidth = this.LineWidth - PRICE_CHARS - offset - this.Currency.Length - 1, len = text.Length;
        string offsetString = "".PadLeft(offset);
        if (len <= maxWidth) return text.PadRight(maxWidth + offset);
        StringBuilder sb = new();
        int i, firstLine = maxWidth - 1 + offset, lines = (text.Length - firstLine) / (maxWidth - 1);
        sb.AppendLine(string.Concat(text.AsSpan(0, firstLine), "~"));
        for (i = 0; i < lines; i++)
            sb.Append(offsetString).AppendLine(string.Concat(text.AsSpan(firstLine + i * (maxWidth - 1), maxWidth - 1), "~"));
        sb.Append(offsetString).Append(text.Substring(firstLine + i * (maxWidth - 1)).PadRight(maxWidth));
        return sb.ToString();
    }
    public string FormatPrice(int price)
    {
        return (this.Currency + ((float)price / 100).ToString("0.00")).PadLeft(PRICE_CHARS + this.Currency.Length + 1);
    }
}
