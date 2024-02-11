using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gsmarena.WindowsApplication.Models.Entities;

namespace Gsmarena.WindowsApplication.Models.ViewModels;

public class MainVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private IList<Device> _devices = new List<Device>();
    
    public IEnumerable<string> Devices
    {
        get { return _devices.Select(device => device.Name); }
    }

    public Device Choose
    {
        get;
        private set;
    }

    private string _selected = string.Empty;

    public string Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            Choose = _devices.First(device => device.Name.Equals(value));
            OnPropertyChanged(nameof(Choose));
            OnPropertyChanged(nameof(Selected));
        }
    }

    public void AddDevice(Device item)
    {
        _devices.Add(item);

        OnPropertyChanged(nameof(Devices));
    }
}