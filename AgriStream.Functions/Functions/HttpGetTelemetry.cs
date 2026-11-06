using System.Net;
using AgriStream.Functions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AgriStream.Functions.Functions;

public class HttpGetTelemetry
{
    private readonly ILogger<HttpGetTelemetry> _logger;

    public HttpGetTelemetry(ILogger<HttpGetTelemetry> logger)
    {
        _logger = logger;
    }

    [Function("GetLatestTelemetry")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "telemetry/latest")] HttpRequestData req,
        [TableInput("Telemetry", Connection = "AzureWebJobsStorage")] TableClient tableClient)
    {
        _logger.LogInformation("HTTP GET telemetry requested");

        // Query recent entities (simple approach – for production use better partitioning)
        var entities = new List<TelemetryEntity>();
        await foreach (var page in tableClient.QueryAsync<TelemetryEntity>(maxPerPage: 50).AsPages())
        {
            entities.AddRange(page.Values);
        }

        var latest = entities
            .OrderByDescending(e => e.RecordedAt)
            .Take(20)
            .ToList();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(latest);
        return response;
    }
}
