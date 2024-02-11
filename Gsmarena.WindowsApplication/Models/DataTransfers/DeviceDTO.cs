using System;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.DataTransfers;

public class DeviceDTO
{
    public int BrandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal PixelPerInch { get; set; }
    public decimal DisplayRatio { get; set; }
    public decimal DisplaySize { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int BatteryCapacity { get; set; }
    public string CpuModel { get; set; } = string.Empty;
    public string CountOfThread { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int OperationSystemId { get; set; }
    public string OperationSystemVersion { get; set; } = string.Empty;
}