namespace SplitBill;

public sealed partial class MainPage : Page
{
    internal ViewModels.MainPageViewModel ViewModel { get; }
    public MainPage()
    {
        this.ViewModel = new ViewModels.MainPageViewModel();
        this.ViewModel.ReceiptNavigationRequested += OnNavigateToReceipt;
        this.InitializeComponent();
        this.ReceiptList.ItemClick += this.OnReceiptClicked;
    }

    private void OnReceiptClicked(object _, ItemClickEventArgs e) =>
        this.ViewModel.OpenReceiptCommand.Execute(e.ClickedItem as Data.Receipt); // Can't figure out how to achieve this through pure commanding

    private void OnNavigateToReceipt(Data.Receipt receipt)
    {
        this.Frame.Navigate(typeof(Views.ReceiptPage), receipt);
    }
}
