namespace Gsmarena.WindowsApplication.Models.Entities;

public class MemoryCapacity
{
    public string UnitOfInternal { get; set; } = string.Empty;
    public float SizeOfInternal { get; set; }
    public string UnitOfRam { get; set; } = string.Empty;
    public float? SizeOfRam { get; set; }
}