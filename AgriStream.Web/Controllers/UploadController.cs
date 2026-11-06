
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace AgriStream.Web.Controllers;

public class UploadController : Controller
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<UploadController> _logger;

    public UploadController(
        BlobServiceClient blobServiceClient,
        ILogger<UploadController> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDroneImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError(
                "",
                "Please select a drone image.");

            return View("Index");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        var extension = Path.GetExtension(file.FileName);

        if (!allowedExtensions.Contains(
            extension,
            StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                "",
                "Only JPG and PNG images are allowed.");

            return View("Index");
        }

        if (file.Length > 10 * 1024 * 1024)
        {
            ModelState.AddModelError(
                "",
                "Maximum upload size is 10 MB.");

            return View("Index");
        }

        try
        {
            var container =
                _blobServiceClient.GetBlobContainerClient("drone-images");

            await container.CreateIfNotExistsAsync();

            var blobName =
                $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";

            var blobClient = container.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                overwrite: false);

            _logger.LogInformation(
                "Drone image uploaded: {BlobName}",
                blobName);

            TempData["Message"] =
                $"Image uploaded successfully: {blobName}. " +
                "Azure BlobTriggerAudit will process it automatically.";

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Drone image upload failed.");

            ModelState.AddModelError(
                "",
                "Image upload failed. Please try again.");

            return View("Index");
        }
    }
}
