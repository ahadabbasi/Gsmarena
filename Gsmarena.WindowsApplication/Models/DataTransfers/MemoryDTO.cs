namespace Gsmarena.WindowsApplication.Models.DataTransfers;

public class MemoryDTO
{
    public int DeviceId { get; set; }
    public decimal MemorySize { get; set; }
    public string MemoryUnit { get; set; } = string.Empty;
    public decimal? RamSize { get; set; }
    public string? RamUnit { get; set; }
    
}