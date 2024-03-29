﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Gsmarena.WindowsApplication.Models.Attributes;
using Gsmarena.WindowsApplication.Models.Entities;

namespace Gsmarena.WindowsApplication.Models.ViewModels;

public class MainVM : INotifyPropertyChanged, IEnumerable<Device>
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private IDictionary<int, IList<Device>> _devices = new Dictionary<int, IList<Device>>();

    private int Key { get; set; } = 0;
    private int Count { get; set; } = 0;
    
    private string SearchKey
    {
        get { return "Has problem at"; }
    }
    
    public IEnumerable<string> Devices
    {
        get;
        private set;
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
            Choose = _devices.SelectMany(pair => pair.Value).FirstOrDefault(device => device.Name.Equals(value));
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

            if (value.GetType() == typeof(string[]))
            {
                string concat = string.Join(string.Empty, (string [])value);
                if (concat.Length == 0)
                {
                    ChangeItemName(item, propertyName);
                    break;
                }
            }

            if (value.ToString() == "0")
            {
                ChangeItemName(item, propertyName);
                break;
            }
        }

        if (!_devices.ContainsKey(Key))
        {
            _devices.Add(Key, new List<Device>());
        }
        
        _devices[Key].Add(item);

        if (_devices[Key].Count == 300)
        {
            Key++;
        }

        if (item.Name.Contains(SearchKey) == false)
        {
            Count++;
        }
        
        Devices = _devices.SelectMany(device => device.Value)
            .Select(device => device.Name);

        OnPropertyChanged(nameof(Devices));
    }

    private void ChangeItemName(Device item, string propertyName)
    {
        item.Name = $"{item.Name} - {SearchKey} {propertyName}";
    }

    public void Delete(string name)
    {
        Devices = _devices.SelectMany(device => device.Value)
            .Where(device => device.Name != name)
            .Select(device => device.Name);
        
        OnPropertyChanged(nameof(Devices));
        Selected = string.Empty;
    }

    public IEnumerator<Device> GetEnumerator()
    {
        return _devices.SelectMany(pair => pair.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear()
    {
        _devices.Clear();
        
        OnPropertyChanged(nameof(Devices));
    }

    public int GetCout()
    {
        return Count;
    }
}