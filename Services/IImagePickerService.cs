using Microsoft.Maui.Storage;

namespace ConstructionStore.Admin.Services;

public interface IImagePickerService
{
    /// <summary>
    /// Opens the native image picker and returns the selected files,
    /// or null if the user cancelled.
    /// </summary>
    Task<IReadOnlyList<FileResult>?> PickImagesAsync();
}

public sealed class ImagePickerService : IImagePickerService
{
    private static readonly FilePickerFileType ImageFileTypes = new FilePickerFileType(
        new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.Android,      new[] { "image/jpeg", "image/png", "image/webp", "image/gif", "image/heic", "image/heif" } },
            { DevicePlatform.iOS,          new[] { "public.image" } },
            { DevicePlatform.MacCatalyst,  new[] { "public.image" } },
            { DevicePlatform.WinUI,        new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".heic", ".heif" } },
        });

    public async Task<IReadOnlyList<FileResult>?> PickImagesAsync()
    {
        try
        {
            var results = await FilePicker.Default.PickMultipleAsync(new PickOptions
            {
                FileTypes = ImageFileTypes,
                PickerTitle = "Select images"
            });

            return results?.ToList();
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }
}
