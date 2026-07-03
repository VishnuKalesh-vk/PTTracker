using Microsoft.Extensions.Options;
using PTTracker.Application.Common.Interfaces;

namespace PTTracker.Infrastructure.Storage;

/// <summary>Writes blobs to the local filesystem under a configured base path. Swap for cloud storage later.</summary>
public class LocalFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    private readonly string _basePath = options.Value.BasePath;

    public async Task<string> SaveAsync(string relativeKey, byte[] content, CancellationToken cancellationToken = default)
    {
        var normalizedKey = relativeKey.Replace('\\', '/');
        var fullPath = Path.Combine(_basePath, normalizedKey.Replace('/', Path.DirectorySeparatorChar));

        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
        return normalizedKey;
    }
}
