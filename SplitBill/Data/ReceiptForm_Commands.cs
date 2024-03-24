namespace SplitBill.Data;

internal partial class ReceiptForm
{
    [RelayCommand]
    private void AddItem()
    {
        this.Items.Add(new ReceiptItem(this, "<new>", 0));
    }
    [RelayCommand]
    private void AddParticipant()
    {
        this.Names.Add("Person " + (this.Names.Count + 1));
    }
    public ReceiptItem SelectedItem { get => this._selected; set { this._selected = value; this.OnPropertyChanged(); } }
    private ReceiptItem _selected;
    [RelayCommand]
    private void RemoveItem()
    {
        this.Items.Remove(this.SelectedItem);
    }
}
