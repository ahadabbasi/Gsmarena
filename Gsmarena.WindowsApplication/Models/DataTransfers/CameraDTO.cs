namespace Gsmarena.WindowsApplication.Models.DataTransfers;

public class CameraDTO
{
    public decimal Pixel { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int DeviceId { get; set; }
}