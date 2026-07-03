namespace PTTracker.Application.Common.Interfaces;

public interface IFileStorage
{
    /// <summary>Persists bytes under <paramref name="relativeKey"/> and returns the stored key.</summary>
    Task<string> SaveAsync(string relativeKey, byte[] content, CancellationToken cancellationToken = default);
}
