using System.Diagnostics.Eventing.Reader;
using SplitBill.Data;
using Windows.Security.Authentication.OnlineId;

namespace SplitBill.Models;
internal static class ReceiptStore
{
    private static StorageFolder? _receipts;
    /// <summary>
    /// Open or create the app's folder in the Documents library.
    /// </summary>
    private static async Task<StorageFolder> GetStorage()
    {
        if (_receipts is null)
        {
            StorageFolder documents;
            try
            {
                documents = (await StorageLibrary.GetLibraryAsync(KnownLibraryId.Documents)).SaveFolder;
            }
            catch (NotImplementedException)
            {
                // Libraries don't seem to be implemented on Linux, so we fall back to home directory
                if (Path.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)))
                    documents = await StorageFolder.GetFolderFromPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                else
                {
                    documents = await StorageFolder.GetFolderFromPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                    documents = await documents.CreateFolderAsync("Documents", CreationCollisionOption.OpenIfExists);
                }
            }
            _receipts = await documents.CreateFolderAsync("SplitBill", CreationCollisionOption.OpenIfExists);
        }
        return _receipts;
    }
    /// <summary>
    /// Loads receipts from app's folder in the Documents library.
    /// </summary>
    /// <returns><see cref="IEnumerable"/> of all found <see cref="Receipt"/>s</returns>
    public static async Task<IEnumerable<Receipt>> LoadReceiptsAsync()
    {
        StorageFolder folder = await GetStorage();
        //System.IO.Directory.EnumerateFiles(folder.Path);
        IEnumerable<StorageFile> files = await folder.GetFilesAsync(); // FIXME: This either crashes somewhere, or it doesn't return anything
        return files.Select(f => new Receipt(f)).OrderBy(r => r.Date);
    }
    /// <summary>
    /// Creates a new file for a <see cref="Receipt"/>.
    /// Does not save any data inside the file.
    /// </summary>
    /// <param name="title">Title of the receipt</param>
    /// <param name="date">Date (and time) of the purchase</param>
    /// <returns>A blank <see cref="Receipt"/></returns>
    public static async Task<Receipt> CreateReceiptAsync(string title, DateTime date)
    {
        StorageFolder folder = await GetStorage();
        // FIXME: Replace forbidden characters in title for filename
        StorageFile file = await folder.CreateFileAsync($"{date:yyyyMMddHHmmss}-{title.ToLowerInvariant()}.txt", CreationCollisionOption.OpenIfExists);
        return new Receipt(file, title, date);
    }
}
