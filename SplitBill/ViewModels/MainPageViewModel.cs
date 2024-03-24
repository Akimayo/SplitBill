using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SplitBill.Data;
using SplitBill.Models;

namespace SplitBill.ViewModels;
internal partial class MainPageViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Receipt> Receipts { get => _receipts; }
    private readonly ObservableCollection<Receipt> _receipts = new();
    public bool IsLoaded { get => this._loaded; private set { this._loaded = value; this.OnPropertyChanged(); } }
    private bool _loaded = false;
    public string ReceiptTitle { get => this._receiptTitle; set { this._receiptTitle = value; this.OnPropertyChanged(); } }
    private string _receiptTitle = "";
    public DateTimeOffset ReceiptDate { get => this._receiptDate; set { this._receiptDate = value; this.OnPropertyChanged(); } }
    private DateTimeOffset _receiptDate = DateTimeOffset.Now;

    public MainPageViewModel()
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        this.ConstructAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private async Task ConstructAsync()
    {
        this.Receipts.AddRange(await ReceiptStore.LoadReceiptsAsync());
        this.IsLoaded = true;
    }

    private void OnPropertyChanged([CallerMemberName] string propName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    public event PropertyChangedEventHandler? PropertyChanged;
    private void NavigateToReceipt(Receipt receipt) => this.ReceiptNavigationRequested?.Invoke(receipt);
    internal delegate void ReceiptNavigationRequestedEventHandler(Receipt receipt);
    internal event ReceiptNavigationRequestedEventHandler? ReceiptNavigationRequested;
}
