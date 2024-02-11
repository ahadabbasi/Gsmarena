using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

    [JsonIgnore]
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

    [JsonIgnore]
    private static string FilePath
    {
        get
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{nameof(Database)}.json");
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

    public static DatabaseVM Read()
    {
        DatabaseVM result = new DatabaseVM();
        
        if (File.Exists(FilePath))
        {
            string fileContent = File.ReadAllText(FilePath);
            result = JsonSerializer.Deserialize<DatabaseVM>(fileContent, new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) ?? result;
        }

        return result;
    }
}