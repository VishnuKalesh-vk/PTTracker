namespace PTTracker.Application.Common.Models;

public class EmailIngestionOptions
{
    public const string SectionName = "EmailIngestion";

    public int SubjectFallbackWindowDays { get; init; } = 30;
    public long MaxFileSizeBytes { get; init; } = 36_700_160; // 35 MB
    public string[] AllowedAttachmentExtensions { get; init; } = { ".xlsx", ".csv" };
}
