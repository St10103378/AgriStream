using Azure;
using Azure.Data.Tables;

namespace AgriStream.Functions.Models;

public class AuditEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "";

    public string RowKey { get; set; } = "";

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public string FileName { get; set; } = "";

    public DateTime UploadedAt { get; set; }

    public string Status { get; set; } = "";

    public string Message { get; set; } = "";
}