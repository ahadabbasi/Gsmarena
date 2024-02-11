using System;
using System.Collections.Generic;
using System.Linq;

namespace Gsmarena.WindowsApplication.Models.Entities;

public class Device
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public IEnumerable<MemoryCapacity> Memories { get; set; } = new MemoryCapacity[] { };

    public DeviceType Type { get; set; }
    public IEnumerable<string> Networks { get; set; } = new string[] { };
    public float DisplaySize { get; set; }
    public float DisplayRatio { get; set; }
    public float PixelPerInch { get; set; }
    public int BatteryCapacity { get; set; }
    public float? Weight { get; set; }
    public string CpuModel { get; set; } = string.Empty;
    public string CountOfThread { get; set; } = string.Empty;
    public string OperationSystem { get; set; } = string.Empty;
    public string OperationSystemVersion { get; set; } = string.Empty;

    public float Price { get; set; }

    public IEnumerable<string> Technologies { get; set; } = new string[] { };

    public IEnumerable<string> MemoryCapacities
    {
        get
        {
            return Memories.Select(memory => string.Format(
                "{0} {1}{2}",
                memory.SizeOfInternal,
                memory.UnitOfInternal,
                memory.SizeOfRam == null
                    ? string.Empty
                    : string.Format(
                        " - {0} {1} RAM", memory.SizeOfRam, memory.UnitOfRam
                    )
            ));
        }
    }

    public IEnumerable<string> CamerasDescription
    {
        get
        {
            return Cameras.Select(camera => string.Format(
                "{0} MP {1} - {2}",
                camera.Pixel,
                !string.IsNullOrEmpty(camera.Type) ? $"({camera.Type})" : string.Empty,
                camera.Position.ToString()
            ));

        }
    }

    public Dimension Dimension { get; set; } = new Dimension();
    public IEnumerable<Camera> Cameras { get; set; } = new Camera[] { };
}