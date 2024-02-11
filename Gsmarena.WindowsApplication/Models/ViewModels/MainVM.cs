using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Gsmarena.WindowsApplication.Models.Attributes;
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

    public Device? Choose
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
            Choose = _devices.FirstOrDefault(device => device.Name.Equals(value));
            OnPropertyChanged(nameof(Choose));
            OnPropertyChanged(nameof(Selected));
        }
    }

    public void AddDevice(Device item)
    {
        foreach (PropertyInfo property in typeof(Device)
                     .GetProperties()
                     .Where(property => property.GetCustomAttributes()
                         .Any(attribute => 
                             attribute.GetType() == typeof(PropertyValidationAttribute)
                             )
                     )
                 )
        {
            string propertyName = property.Name;

            if (property.GetCustomAttributes().Any(attribute => attribute.GetType() == typeof(DisplayAttribute)))
            {
                propertyName = property.GetCustomAttribute<DisplayAttribute>()?.Name ?? propertyName;
            }
            
            object? value = property.GetValue(item);
            if (value == null || 
                (value.GetType() != typeof(IEnumerable<string>) && value.ToString().ToLower() == "UNKNOWN".ToLower()))
            {
                ChangeItemName(item, propertyName);
                break;
            }

            if (value.GetType() == typeof(IEnumerable<string>) && (string.IsNullOrEmpty((string.Join(string.Empty, value)))))
            {
                ChangeItemName(item, propertyName);
                break;
            }
        }
        _devices.Add(item);

        OnPropertyChanged(nameof(Devices));
    }

    private void ChangeItemName(Device item, string propertyName)
    {
        item.Name = $"{item.Name} - Has problem at {propertyName}";
    }

    public void Delete(Device item)
    {
        _devices.Remove(item);
        OnPropertyChanged(nameof(Devices));
        Selected = string.Empty;
    }
}