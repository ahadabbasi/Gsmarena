using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Gsmarena.WindowsApplication.Models.Attributes;

namespace Gsmarena.WindowsApplication.Models.Entities;

public class Device
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    
    [
        PropertyValidation,
        Display(Name = "Release Date")
    ]
    public DateTime? ReleaseDate { get; set; }
    
    public IEnumerable<MemoryCapacity> Memories { get; set; } = new MemoryCapacity[] { };

    public DeviceType Type { get; set; }
    
    [
        PropertyValidation
    ]
    public IEnumerable<string> Networks { get; set; } = new string[] { };
    
    [
        PropertyValidation,
        Display(Name = "Display Size")
    ]
    public float DisplaySize { get; set; }
    
    [
        PropertyValidation,
        Display(Name = "Display Ratio")
    ]
    public float DisplayRatio { get; set; }
    
    [
        PropertyValidation,
        Display(Name = "Display Pixel Per Inch")
    ]
    public float PixelPerInch { get; set; }
    
    [
        PropertyValidation,
        Display(Name = "Battery Capacity")
    ]
    public int BatteryCapacity { get; set; }
    
    [PropertyValidation]
    public float? Weight { get; set; }
    
    [
        PropertyValidation,
        Display(Name = "Cpu Model")
    ]
    public string CpuModel { get; set; } = string.Empty;
    
    [
        PropertyValidation,
        Display(Name = "Cpu Count Of Thread")
    ]
    public string CountOfThread { get; set; } = string.Empty;
    
    [
        PropertyValidation,
        Display(Name = "Operation System Name")
    ]
    public string OperationSystem { get; set; } = string.Empty;
    
    [
        PropertyValidation,
        Display(Name = "Operation System Version")
    ]
    public string OperationSystemVersion { get; set; } = string.Empty;

    //[PropertyValidation]
    public float Price { get; set; }

    [PropertyValidation]
    public IEnumerable<string> Technologies { get; set; } = new string[] { };

    [
        PropertyValidation,
        Display(Name = "Memory")
    ]
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

    [
        PropertyValidation,
        Display(Name = "Camera")
    ]
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