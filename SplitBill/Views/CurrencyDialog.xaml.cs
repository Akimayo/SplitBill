using SplitBill.Data;

namespace SplitBill.Views;
public sealed partial class CurrencyDialog : ContentDialog
{
    internal ReceiptForm Receipt { get; }
    private string _originalCurrency;
    internal CurrencyDialog(ReceiptForm receipt)
    {
        this.Receipt = receipt;
        this._originalCurrency = receipt.Currency;
        this.InitializeComponent();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        this.Hide();
    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        this.Receipt.Currency = this._originalCurrency;
        this.Hide();
    }
}
