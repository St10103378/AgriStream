using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AgriStream.Functions.Functions;

public class QueueTriggerIrrigation
{
    private readonly ILogger<QueueTriggerIrrigation> _logger;
    private readonly IConfiguration _config;

    public QueueTriggerIrrigation(ILogger<QueueTriggerIrrigation> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [Function("ProcessIrrigation")]
    public async Task Run(
        [QueueTrigger("irrigation-commands", Connection = "AzureWebJobsStorage")] string message)
    {
        _logger.LogInformation("Queue trigger processed irrigation command: {Message}", message);

        string? deviceId = null;
        string action = "start";

        try
        {
            using var doc = JsonDocument.Parse(message);
            if (doc.RootElement.TryGetProperty("deviceId", out var idProp))
                deviceId = idProp.GetString();
            if (doc.RootElement.TryGetProperty("action", out var actionProp))
                action = actionProp.GetString() ?? "start";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse queue message");
            return;
        }

        if (string.IsNullOrWhiteSpace(deviceId))
        {
            _logger.LogWarning("No deviceId in message – skipping");
            return;
        }

        var sqlConn = _config["SqlConnection"] 
                      ?? _config.GetConnectionString("SqlConnection")
                      ?? throw new InvalidOperationException("SqlConnection missing");

        await using var conn = new SqlConnection(sqlConn);
        await conn.OpenAsync();

        var newStatus = action.Equals("start", StringComparison.OrdinalIgnoreCase) ? "Irrigating" : "Idle";

        var cmd = new SqlCommand(@"
            UPDATE Devices 
            SET Status = @Status, 
                LastCommand = @Command, 
                LastUpdated = GETUTCDATE()
            WHERE DeviceId = @DeviceId", conn);

        cmd.Parameters.AddWithValue("@Status", newStatus);
        cmd.Parameters.AddWithValue("@Command", action);
        cmd.Parameters.AddWithValue("@DeviceId", deviceId);

        var rows = await cmd.ExecuteNonQueryAsync();
        _logger.LogInformation("Updated {Rows} device(s). Device {DeviceId} → {Status}", rows, deviceId, newStatus);
    }
}
