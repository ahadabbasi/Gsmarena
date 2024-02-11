using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

    private string _brand = "";

    public string Brand
    {
        get { return _brand; }
        set
        {
            _brand = value;
            OnPropertyChanged(nameof(Brand));
        }
    }
    
    private string _url = "B";

    public string Url
    {
        get { return _url; }
        set
        {
            _url = value;
            OnPropertyChanged(nameof(Url));
        }
    }
    
    private string _name = "C";

    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
    
    private string _networks = "D";

    public string Networks
    {
        get { return _networks; }
        set
        {
            _networks = value;
            OnPropertyChanged(nameof(Networks));
        }
    }
    
    private string _operationSystem = "S";

    public string OperationSystem
    {
        get { return _operationSystem; }
        set
        {
            _operationSystem = value;
            OnPropertyChanged(nameof(Networks));
        }
    }
    
    private string _cpuModel = "U";

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
    private string _countOfThread = "V";

    public string CountOfThread
    {
        get { return _countOfThread; }
        set
        {
            _countOfThread = value;
            OnPropertyChanged(nameof(CountOfThread));
        }
    }
    
    private string _displaySize = "M";

    public string DisplaySize
    {
        get { return _displaySize; }
        set
        {
            _displaySize = value;
            OnPropertyChanged(nameof(DisplaySize));
        }
    }
    
    private string _batteryCapacity = "AS";

    public string BatteryCapacity
    {
        get { return _batteryCapacity; }
        set
        {
            _batteryCapacity = value;
            OnPropertyChanged(nameof(BatteryCapacity));
        }
    }
    
    private string _weight = "I";

    public string Weight
    {
        get { return _weight; }
        set
        {
            _weight = value;
            OnPropertyChanged(nameof(Weight));
        }
    }
    
    private string _operationSystemVersion = "T";

    public string OperationSystemVersion
    {
        get { return _operationSystemVersion; }
        set
        {
            _operationSystemVersion = value;
            OnPropertyChanged(nameof(OperationSystemVersion));
        }
    }
    
    private string _dimension = "H";

    public string Dimension
    {
        get { return _dimension; }
        set
        {
            _dimension = value;
            OnPropertyChanged(nameof(Dimension));
        }
    }
    
    
    private string _displayRatio = "O";

    public string DisplayRatio
    {
        get { return _displayRatio; }
        set
        {
            _displayRatio = value;
            OnPropertyChanged(nameof(DisplayRatio));
        }
    }
    
    
    private string _resolution = "P";

    public string Resolution
    {
        get { return _resolution; }
        set
        {
            _resolution = value;
            OnPropertyChanged(nameof(Resolution));
        }
    }
    
    private string _releaseDate = "F";

    public string ReleaseDate
    {
        get { return _releaseDate; }
        set
        {
            _releaseDate = value;
            OnPropertyChanged(nameof(ReleaseDate));
        }
    }
    
    private string _announceDate = "E";

    public string AnnounceDate
    {
        get { return _announceDate; }
        set
        {
            _announceDate = value;
            OnPropertyChanged(nameof(AnnounceDate));
        }
    }
    
    private string _memoryInternal = "Y";

    public string MemoryInternal
    {
        get { return _memoryInternal; }
        set
        {
            _memoryInternal = value;
            OnPropertyChanged(nameof(MemoryInternal));
        }
    }
    
    private string _mainCamera = "AA";

    public string MainCamera
    {
        get { return _mainCamera; }
        set
        {
            _mainCamera = value;
            OnPropertyChanged(nameof(MainCamera));
        }
    }
    
    private string _selfieCamera = "AE";

    public string SelfieCamera
    {
        get { return _selfieCamera; }
        set
        {
            _selfieCamera = value;
            OnPropertyChanged(nameof(SelfieCamera));
        }
    }
    
    
    private string _technology = "K";

    public string Technology
    {
        get { return _technology; }
        set
        {
            _technology = value;
            OnPropertyChanged(nameof(Technology));
        }
    }
    
    private string _price = "K";

    public string Price
    {
        get { return _price; }
        set
        {
            _price = value;
            OnPropertyChanged(nameof(Price));
        }
    }
}