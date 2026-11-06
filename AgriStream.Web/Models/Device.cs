using System.ComponentModel.DataAnnotations;

namespace AgriStream.Web.Models;

public class Device
{
    [Key]
    [MaxLength(50)]
    public string DeviceId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "Idle";

    [MaxLength(100)]
    public string? LastCommand { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? Location { get; set; }
}
