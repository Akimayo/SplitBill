using SplitBill.Data;
using SplitBill.Models;

namespace SplitBill.ViewModels;
internal partial class MainPageViewModel
{
    [RelayCommand]
    private async Task CreateReceipt()
    {
        if (string.IsNullOrEmpty(this.ReceiptTitle)) return;
        Receipt r = await ReceiptStore.CreateReceiptAsync(this.ReceiptTitle, this.ReceiptDate.UtcDateTime);
        this.NavigateToReceipt(r);
    }
    [RelayCommand]
    private void OpenReceipt(Receipt r)
    {
        this.NavigateToReceipt(r);
    }
}
