using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SplitBill.Data;
internal class ReceiptItem : INotifyPropertyChanged
{
    public ReceiptForm Receipt { get; }
    public string Name { get => this._name; set { if (this._name != value) { this._name = value; this.OnPropertyChanged(); } } }
    private string _name;
    public int Price { get => this._price; set { if (this._price != value) { this._price = value; this.OnPropertyChanged(); } } }
    private int _price;
    public ObservableCollection<byte> Participants { get; } = new();
    public IEnumerable<ParticipantDAO> ParticipantCounts
    {
        get => this._participantCounts;
        private set { this._participantCounts = value; this.OnPropertyChanged(); this.OnPropertyChanged(nameof(this.ActiveParticipantCounts)); }
    }
    private IEnumerable<ParticipantDAO> _participantCounts;
    public IEnumerable<ParticipantDAO> ActiveParticipantCounts => this._participantCounts.Where(p => p.Amount > 0);

    public ReceiptItem(ReceiptForm parent, string name, int price)
    {
        this.Receipt = parent;
        this._name = name;
        this._price = price;
        parent.Names.CollectionChanged += this.UpdateParticipants;
        this.Participants.CollectionChanged += TriggerDAOUpdate;
        this._participantCounts = this.Receipt.Names.Select((_, i) => new ParticipantDAO(this, i));
    }

    private void TriggerDAOUpdate(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        this.OnPropertyChanged(nameof(this.ParticipantCounts));

    private void UpdateParticipants(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        this.ParticipantCounts = this.Receipt.Names.Select((_, i) => new ParticipantDAO(this, i));

    public int PriceFor(int participant)
    {
        if (this.Participants.Count > participant) return (int)Math.Round(this.Participants[participant] * (float)this.Price / this.Participants.Sum(i => i));
        return 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

}
internal class ParticipantDAO(ReceiptItem parent, int index) : INotifyPropertyChanged
{
    public byte Amount
    {
        get => parent.Participants.Count > index && index >= 0 ? parent.Participants[index] : (byte)0;
        set
        {
            if (index >= parent.Participants.Count)
                parent.Participants.AddRange(new byte[index - parent.Participants.Count + 1]);
            parent.Participants[index] = value;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.ShowAmountInput));
        }
    }
    public string Name
    {
        get => parent.Receipt.Names[index];
        set
        {
            parent.Receipt.Names[index] = value;
            this.OnPropertyChanged();
        }
    }
    public bool ShowAmountInput { get => this.Amount > 0; }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propName = "") => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
