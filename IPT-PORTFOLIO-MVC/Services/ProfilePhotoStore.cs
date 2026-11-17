using System.Security.Cryptography;
using System.Text;

namespace IPT_PORTFOLIO_MVC.Services;

// Private server storage. Files are served through an authenticated controller.
public sealed class ProfilePhotoStore(IWebHostEnvironment environment)
{
    public const int MaximumBytes = 3 * 1024 * 1024;
    private string Folder => Path.Combine(environment.ContentRootPath, "App_Data", "ProfilePhotos");
    private string PhotoPath(string accountId) =>
        Path.Combine(Folder, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(accountId))) + ".photo");

    public bool Exists(string accountId) => File.Exists(PhotoPath(accountId));

    public async Task<string?> SaveAsync(string accountId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0 || file.Length > MaximumBytes)
            return "Choose a photo up to 3 MB.";

        using var buffer = new MemoryStream();
        await file.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();
        if (bytes.Length > MaximumBytes || ContentType(bytes) is null)
            return "Choose a JPG, PNG, or WebP image.";

        Directory.CreateDirectory(Folder);
        var temporaryPath = Path.Combine(Folder, Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            await File.WriteAllBytesAsync(temporaryPath, bytes, cancellationToken);
            File.Move(temporaryPath, PhotoPath(accountId), overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
        return null;
    }

    public async Task<(byte[] Bytes, string ContentType)?> ReadAsync(string accountId, CancellationToken cancellationToken)
    {
        try
        {
            // Allow atomic replacement while another request reads the previous photo.
            await using var file = new FileStream(PhotoPath(accountId), FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete, 4096, useAsync: true);
            if (file.Length > MaximumBytes) return null;
            using var buffer = new MemoryStream();
            await file.CopyToAsync(buffer, cancellationToken);
            var bytes = buffer.ToArray();
            var contentType = ContentType(bytes);
            return contentType is null ? null : (bytes, contentType);
        }
        catch (FileNotFoundException) { return null; }
        catch (DirectoryNotFoundException) { return null; }
    }

    internal static string? ContentType(byte[] bytes)
    {
        if (bytes.Length < 20) return null;
        var data = bytes.AsSpan();
        if (data.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF }) &&
            data.EndsWith(new byte[] { 0xFF, 0xD9 }))
            return "image/jpeg";
        if (data.StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) &&
            data.Slice(bytes.Length - 8, 4).SequenceEqual("IEND"u8))
            return "image/png";
        if (data[..4].SequenceEqual("RIFF"u8) && data.Slice(8, 4).SequenceEqual("WEBP"u8))
            return "image/webp";
        return null;
    }
}
