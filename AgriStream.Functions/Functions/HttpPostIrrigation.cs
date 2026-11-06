using System.Net;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AgriStream.Functions.Functions;

public class HttpPostIrrigation
{
    private readonly ILogger<HttpPostIrrigation> _logger;
    private readonly QueueServiceClient _queueServiceClient;

    public HttpPostIrrigation(ILogger<HttpPostIrrigation> logger, QueueServiceClient queueServiceClient)
    {
        _logger = logger;
        _queueServiceClient = queueServiceClient;
    }

    [Function("TriggerIrrigation")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "irrigation/trigger")] HttpRequestData req)
    {
        _logger.LogInformation("HTTP POST irrigation trigger received");

        var body = await new StreamReader(req.Body).ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Request body is required");
            return bad;
        }

        // Basic validation
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("deviceId", out _))
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("deviceId is required");
                return bad;
            }
        }
        catch
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid JSON");
            return bad;
        }

        // Send message to the queue
        var queueClient = _queueServiceClient.GetQueueClient("irrigation-commands");
        await queueClient.CreateIfNotExistsAsync();
        await queueClient.SendMessageAsync(body);

        _logger.LogInformation("Irrigation command enqueued successfully");

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteAsJsonAsync(new { status = "queued", message = "Irrigation command accepted" });
        return response;
    }
}