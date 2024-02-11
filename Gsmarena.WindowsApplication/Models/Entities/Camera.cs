namespace Gsmarena.WindowsApplication.Models.Entities;

public class Camera
{
    public float Pixel { get; set; }
    public string Type { get; set; } = string.Empty;
    public CameraPosition Position { get; set; }

}