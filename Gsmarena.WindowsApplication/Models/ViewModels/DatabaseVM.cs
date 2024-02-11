using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.ViewModels;

public class DatabaseVM : INotifyPropertyChanged
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

    private string _server;

    public string Server
    {
        get { return _server; }
        set
        {
            _server = value;
            OnPropertyChanged(nameof(Server));
        }
    }
    
    
    private string _username;

    public string Username
    {
        get { return _username; }
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }
    
    
    private string _password;

    public string Password
    {
        get { return _password; }
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }
    
    
    private string _database;

    public string Database
    {
        get { return _database; }
        set
        {
            _database = value;
            OnPropertyChanged(nameof(Database));
        }
    }

    public string ConnectionString
    {
        get
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
            {
                Server = _server,
                Database = _database,
                UserID = _username,
                Password = _password
            };

            return builder.ConnectionString;
        }
    }
}