
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace AgriStream.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public HomeController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Dashboard()
    {
        ViewBag.Message =
            "Field Dashboard – latest telemetry retrieved from Azure Function.";

        try
        {
            var telemetryUrl =
                _configuration["Functions:TelemetryGetUrl"];

            if (string.IsNullOrWhiteSpace(telemetryUrl))
            {
                throw new InvalidOperationException(
                    "TelemetryGetUrl is missing from configuration.");
            }

            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(telemetryUrl);

            response.EnsureSuccessStatusCode();

            var telemetryJson =
                await response.Content.ReadAsStringAsync();

            ViewBag.TelemetryJson = telemetryJson;
        }
        catch (Exception ex)
        {
            ViewBag.TelemetryJson = "[]";
            ViewBag.Error = ex.Message;
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
