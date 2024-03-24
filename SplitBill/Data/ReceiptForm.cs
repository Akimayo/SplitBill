using System.Collections.ObjectModel;

namespace SplitBill.Data;

internal partial class ReceiptForm : Receipt
{
    public static ReceiptForm Blank => new();
    public bool IsSaving { get => this._isSaving; set { this._isSaving = value; this.OnPropertyChanged(); } }
    private bool _isSaving = true;
    public ObservableCollection<string> Names { get; } = new();
    public ObservableCollection<ReceiptItem> Items { get; } = new();
    public string TextReceipt => this.ToString();
    public int TotalPrice => this.Items.Sum(i => i.Price);

    private DispatcherTimer _timer;

    public ReceiptForm(StorageFile location) : base(location)
    {
        this._timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 5) };
        this._timer.Tick += this.SaveReceipt;
        this.PropertyChanged += this.QueueSaving;
        this.Items.CollectionChanged += this.QueueSaving;
        this.Names.CollectionChanged += this.QueueSaving;
    }

    private ReceiptForm() : base(null, "<blank file>", DateTime.MinValue) { }

    private void QueueSaving(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        this.IsSaving = true;
        this._timer.Stop();
        this._timer.Start();
        if (sender != null && sender.Equals(this.Items))
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null) foreach (ReceiptItem i in e.NewItems)
                        {
                            i.PropertyChanged += this.QueueSaving;
                            i.Participants.CollectionChanged += this.QueueSaving;
                        }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null) foreach (ReceiptItem i in e.OldItems)
                        {
                            i.PropertyChanged -= this.QueueSaving;
                            i.Participants.CollectionChanged -= this.QueueSaving;
                        }
                    break;
            }
    }

    private void QueueSaving(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(this.Currency):
            case nameof(this.Title):
            case nameof(this.Date):
            case nameof(this.LineWidth):
            case nameof(ReceiptItem.Name):
            case nameof(ReceiptItem.Price):
            case nameof(ReceiptItem.Participants):
                this.IsSaving = true;
                this._timer.Stop();
                this._timer.Start();
                break;
        }
        if (e.PropertyName == nameof(ReceiptItem.Price)) this.OnPropertyChanged(nameof(this.TotalPrice));
    }
}
