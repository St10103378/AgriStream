using AgriStream.Functions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AgriStream.Functions.Functions;

public class BlobTriggerAudit
{
    private readonly ILogger<BlobTriggerAudit> _logger;

    public BlobTriggerAudit(ILogger<BlobTriggerAudit> logger)
    {
        _logger = logger;
    }

    [Function("BlobTriggerAudit")]
    public async Task Run(
        [BlobTrigger(
            "drone-images/{name}",
            Connection = "AzureWebJobsStorage")]
        Stream blobStream,
        string name,

        [TableInput(
            "AuditLogs",
            Connection = "AzureWebJobsStorage")]
        TableClient tableClient)
    {
        _logger.LogInformation(
            "Processing drone image: {Name}, Size: {Length}",
            name,
            blobStream.Length);

        var entity = new AuditEntity
        {
            PartitionKey = "Upload",
            RowKey = Guid.NewGuid().ToString("N"),
            FileName = name,
            UploadedAt = DateTime.UtcNow,
            Status = "Received",
            Message = $"Blob size: {blobStream.Length} bytes"
        };

        await tableClient.AddEntityAsync(entity);

        _logger.LogInformation(
            "Audit record successfully saved for {Name}",
            name);
    }
}