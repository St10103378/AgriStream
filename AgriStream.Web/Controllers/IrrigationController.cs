using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AgriStream.Web.Controllers;

public class IrrigationController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<IrrigationController> _logger;

    public IrrigationController(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<IrrigationController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Trigger(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            ModelState.AddModelError("", "Device ID is required");
            return View("Index");
        }

        var functionUrl = _config["Functions:IrrigationPostUrl"]; 
        if (string.IsNullOrEmpty(functionUrl))
        {
            TempData["Error"] = "Function URL not configured";
            return RedirectToAction("Index");
        }

        var client = _httpClientFactory.CreateClient();
        var payload = JsonSerializer.Serialize(new { deviceId, action = "start", requestedAt = DateTime.UtcNow });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(functionUrl, content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = $"Irrigation command queued for device {deviceId}";
        }
        else
        {
            TempData["Error"] = $"Failed to queue command: {response.StatusCode}";
        }

        return RedirectToAction("Index");
    }
}
