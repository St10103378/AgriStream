using Azure;
using Azure.Data.Tables;

namespace AgriStream.Functions.Models;

public class TelemetryEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Sensor";
    public string RowKey { get; set; } = string.Empty;   // usually deviceId + timestamp
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string DeviceId { get; set; } = string.Empty;
    public double SoilMoisture { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public DateTime RecordedAt { get; set; }
}
