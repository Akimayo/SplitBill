using System.Collections.ObjectModel;
using System.ComponentModel;
using SplitBill.Data;

namespace SplitBill.Views;
public sealed partial class ParticipantsDialog : ContentDialog
{
    private ReceiptForm Receipt { get; }
    internal ObservableCollection<NameDAO> Names { get; }
    internal ParticipantsDialog(ReceiptForm receipt)
    {
        this.Receipt = receipt;
        this.Names = new(receipt.Names.Select(s => new NameDAO(s)));
        this.InitializeComponent();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        this.Receipt.Names.Clear();
        this.Receipt.Names.AddRange(this.Names.Select(s => s.Name));
        this.Hide();
    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        this.Hide();
    }

    internal sealed class NameDAO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Name { get => this._name; set { this._name = value; this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Name))); } }
        private string _name = "";
        public NameDAO(string name)
        {
            this._name = name;
        }
    }

    private void OnAddParticipant(object sender, RoutedEventArgs e)
    {
        this.Names.Add(new("Person " + (this.Names.Count + 1)));
    }
}
