using System.Security.Cryptography;
using System.Text;
namespace IPT_PORTFOLIO_MVC.Services;

public sealed class ProjectImageStore(IWebHostEnvironment environment)
{
    public const int MaximumBytes = ProfilePhotoStore.MaximumBytes;
    private string Folder => Path.Combine(environment.ContentRootPath, "App_Data", "ProjectImages");
    private string ImagePath(string accountId, string imageId) =>
        Path.Combine(Folder, Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(accountId + ":" + imageId))) + ".image");

    public async Task<(byte[]? Bytes, string? Error)> ValidateAsync(IFormFile image, CancellationToken cancellationToken)
    {
        if (image.Length == 0 || image.Length > MaximumBytes)
            return (null, "Choose a non-empty image up to 3 MB.");
        using var output = new MemoryStream();
        using var input = image.OpenReadStream();
        var buffer = new byte[8192];
        int count;
        while ((count = await input.ReadAsync(buffer, cancellationToken)) > 0)
        {
            if (output.Length + count > MaximumBytes) return (null, "Choose an image up to 3 MB.");
            output.Write(buffer, 0, count);
        }
        var bytes = output.ToArray();
        return ProfilePhotoStore.ContentType(bytes) is null
            ? (null, "Choose a JPG, PNG, or WebP image.") : (bytes, null);
    }

    public async Task<string> SaveAsync(string accountId, byte[] bytes, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Folder);
        var id = Guid.NewGuid().ToString("N");
        var path = ImagePath(accountId, id);
        try { await File.WriteAllBytesAsync(path, bytes, cancellationToken); }
        catch { if (File.Exists(path)) File.Delete(path); throw; }
        return id;
    }

    public void Delete(string accountId, string imageId)
    {
        if (Guid.TryParseExact(imageId, "N", out _)) File.Delete(ImagePath(accountId, imageId));
    }

    public async Task<(byte[] Bytes, string ContentType)?> ReadAsync(string accountId, string imageId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParseExact(imageId, "N", out _)) return null;
        try
        {
            var path = ImagePath(accountId, imageId);
            if (new FileInfo(path).Length > MaximumBytes) return null;
            var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
            var type = ProfilePhotoStore.ContentType(bytes);
            return type is null ? null : (bytes, type);
        }
        catch (FileNotFoundException) { return null; }
        catch (DirectoryNotFoundException) { return null; }
    }
}
