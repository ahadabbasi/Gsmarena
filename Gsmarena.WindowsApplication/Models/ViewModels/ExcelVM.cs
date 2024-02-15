using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Gsmarena.WindowsApplication.Models.ViewModels;

public class ExcelVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private string _brand = "BA";

    public string Brand
    {
        get { return _brand; }
        set
        {
            _brand = value;
            OnPropertyChanged(nameof(Brand));
        }
    }

    private string _type = "BB";

    public string Type
    {
        get { return _type; }
        set
        {
            _type = value;
            OnPropertyChanged(nameof(Type));
        }
    }

    private string _url = "C";

    public string Url
    {
        get { return _url; }
        set
        {
            _url = value;
            OnPropertyChanged(nameof(Url));
        }
    }

    private string _name = "D";

    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private string _networks = "E";

    public string Networks
    {
        get { return _networks; }
        set
        {
            _networks = value;
            OnPropertyChanged(nameof(Networks));
        }
    }

    private string _operationSystem = "T";

    public string OperationSystem
    {
        get { return _operationSystem; }
        set
        {
            _operationSystem = value;
            OnPropertyChanged(nameof(Networks));
        }
    }

    private string _cpuModel = "V";

    public string CpuModel
    {
        get { return _cpuModel; }
        set
        {
            _cpuModel = value;
            OnPropertyChanged(nameof(CpuModel));
        }
    }

    //DisplaySize
    private string _countOfThread = "W";

    public string CountOfThread
    {
        get { return _countOfThread; }
        set
        {
            _countOfThread = value;
            OnPropertyChanged(nameof(CountOfThread));
        }
    }

    private string _displaySize = "N";

    public string DisplaySize
    {
        get { return _displaySize; }
        set
        {
            _displaySize = value;
            OnPropertyChanged(nameof(DisplaySize));
        }
    }

    private string _batteryCapacity = "AT";

    public string BatteryCapacity
    {
        get { return _batteryCapacity; }
        set
        {
            _batteryCapacity = value;
            OnPropertyChanged(nameof(BatteryCapacity));
        }
    }

    private string _weight = "J";

    public string Weight
    {
        get { return _weight; }
        set
        {
            _weight = value;
            OnPropertyChanged(nameof(Weight));
        }
    }

    private string _operationSystemVersion = "U";

    public string OperationSystemVersion
    {
        get { return _operationSystemVersion; }
        set
        {
            _operationSystemVersion = value;
            OnPropertyChanged(nameof(OperationSystemVersion));
        }
    }

    private string _dimension = "I";

    public string Dimension
    {
        get { return _dimension; }
        set
        {
            _dimension = value;
            OnPropertyChanged(nameof(Dimension));
        }
    }


    private string _displayRatio = "P";

    public string DisplayRatio
    {
        get { return _displayRatio; }
        set
        {
            _displayRatio = value;
            OnPropertyChanged(nameof(DisplayRatio));
        }
    }


    private string _resolution = "Q";

    public string Resolution
    {
        get { return _resolution; }
        set
        {
            _resolution = value;
            OnPropertyChanged(nameof(Resolution));
        }
    }

    private string _releaseDate = "G";

    public string ReleaseDate
    {
        get { return _releaseDate; }
        set
        {
            _releaseDate = value;
            OnPropertyChanged(nameof(ReleaseDate));
        }
    }

    private string _announceDate = "F";

    public string AnnounceDate
    {
        get { return _announceDate; }
        set
        {
            _announceDate = value;
            OnPropertyChanged(nameof(AnnounceDate));
        }
    }

    private string _memoryInternal = "Z";

    public string MemoryInternal
    {
        get { return _memoryInternal; }
        set
        {
            _memoryInternal = value;
            OnPropertyChanged(nameof(MemoryInternal));
        }
    }

    private string _mainCamera = "AB";

    public string MainCamera
    {
        get { return _mainCamera; }
        set
        {
            _mainCamera = value;
            OnPropertyChanged(nameof(MainCamera));
        }
    }

    private string _selfieCamera = "AF";

    public string SelfieCamera
    {
        get { return _selfieCamera; }
        set
        {
            _selfieCamera = value;
            OnPropertyChanged(nameof(SelfieCamera));
        }
    }


    private string _technology = "L";

    public string Technology
    {
        get { return _technology; }
        set
        {
            _technology = value;
            OnPropertyChanged(nameof(Technology));
        }
    }

    private string _price = "AX";

    public string Price
    {
        get { return _price; }
        set
        {
            _price = value;
            OnPropertyChanged(nameof(Price));
        }
    }
    
    private string _sensors = "AR";

    public string Sensors
    {
        get { return _sensors; }
        set
        {
            _sensors = value;
            OnPropertyChanged(nameof(Sensors));
        }
    }

    [JsonIgnore]
    private static string FilePath
    {
        get
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"{nameof(ExcelWindow).Replace(nameof(Window), string.Empty)}.json"
            );
        }
    }

    public async Task SaveAsync()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }

        using (FileStream file = File.Create(FilePath))
        {
            byte[] content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            await file.WriteAsync(content, 0, content.Length);
        }
    }

    public static ExcelVM Read()
    {
        ExcelVM result = new ExcelVM();

        if (File.Exists(FilePath))
        {
            string fileContent = File.ReadAllText(FilePath);
            result = JsonSerializer.Deserialize<ExcelVM>(fileContent, new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) ?? result;
        }

        return result;
    }
}