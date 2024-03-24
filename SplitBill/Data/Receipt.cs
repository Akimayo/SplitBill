using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SplitBill.Data;
internal partial class Receipt : INotifyPropertyChanged
{
    protected bool _initialized = false;
    public string Title { get => this._title; set { this._title = value; OnPropertyChanged(); } }
    private string _title = "";
    public DateTime Date { get => this._date; set { this._date = value; OnPropertyChanged(); } }
    private DateTime _date = DateTime.MinValue;
    public bool IsValid { get => this._isValid; set { this._isValid = value; OnPropertyChanged(); } }
    private bool _isValid = false;
    public string Currency { get => this._currency; set { this._currency = value; OnPropertyChanged(); } }
    private string _currency = "";

    public Receipt(StorageFile location)
    {
        this._location = location;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        LoadMetadataAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        this._initialized = true;
    }

    public Receipt(StorageFile location, string title, DateTime date)
    {
        this._location = location;
        this._title = title;
        this._date = date;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propName = "")
    {
        if (!string.IsNullOrEmpty(propName))
            try
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
            catch (NullReferenceException) { System.Diagnostics.Debug.WriteLine($"Receipt property '{propName}' changed with NullReferenceException"); }
    }
}
