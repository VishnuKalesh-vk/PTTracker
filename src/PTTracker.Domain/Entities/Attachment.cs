using PTTracker.Domain.Common;

namespace PTTracker.Domain.Entities;

public class Attachment : Entity
{
    public Guid MessageId { get; private set; }
    public Message Message { get; private set; } = null!;

    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }

    // Pointer into blob storage (e.g. S3 key or disk path), not the bytes themselves.
    public string StorageKey { get; private set; } = null!;

    private Attachment() { }

    public static Attachment Create(
        Guid messageId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storageKey)
    {
        return new Attachment
        {
            MessageId = messageId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StorageKey = storageKey,
        };
    }
}
