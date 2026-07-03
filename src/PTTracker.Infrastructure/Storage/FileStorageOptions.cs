namespace PTTracker.Infrastructure.Storage;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string BasePath { get; init; } = "storage";
}
