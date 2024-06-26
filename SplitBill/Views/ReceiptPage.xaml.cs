using System.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;
using SplitBill.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SplitBill.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReceiptPage : Page, INotifyPropertyChanged
{
    internal ReceiptForm Receipt { get; private set; } = ReceiptForm.Blank;
    public ReceiptPage()
    {
        this.InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        this.Receipt = await ((Receipt)e.Parameter).LoadAsync();
        base.OnNavigatedTo(e);
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Receipt))); // For Skia+GTK, which renders before OnNavigatedTo
    }

    [RelayCommand]
    private void NaviagateBack()
    {
        this.Frame.GoBack();
    }

    private void DGAddItemInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator _, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        this.Receipt.AddItemCommand.Execute(null);
        if (args.Element is DataGrid dg)
        {
            this.Receipt.SelectedItem = this.Receipt.Items.Last();
            dg.BeginEdit();
        }
    }

    private void DGRemoveItemInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator _, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs __) =>
        this.Receipt.RemoveItemCommand.Execute(null);

    private async void OnChangeCurrencyClick(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new CurrencyDialog(this.Receipt) { XamlRoot = this.XamlRoot };
        await dialog.ShowAsync();
    }
    private async void OnChangeParticipantsClick(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ParticipantsDialog(this.Receipt) { XamlRoot = this.XamlRoot };
        await dialog.ShowAsync();
    }

    private void OnPriceInputLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            tb.Focus(FocusState.Keyboard);
            tb.SelectAll();
        }
    }

    private void DGEditBeginning(object sender, DataGridBeginningEditEventArgs e)
    {
        if (sender is DataGrid dg && e.Column.DisplayIndex == 2)
        {
            dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }
    }

    private void DGEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (sender is DataGrid dg) dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
    }
}
