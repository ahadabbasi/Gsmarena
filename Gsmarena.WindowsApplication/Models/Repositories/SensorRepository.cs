using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.Repositories;

public class SensorRepository
{
    protected string ConnectionString { get; }

    protected string TableName
    {
        get
        {
            return "`sensors`";
        }
    }
    
    protected string ConnectTableName
    {
        get
        {
            return "`devices_sensors`";
        }
    }

    public SensorRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public async Task<int?> GetIdByNameAsync(string name)
    {
        int? result = null;
        
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT `id` FROM {TableName} WHERE `name` = @name";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@name", name));

                await connection.OpenAsync();

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = int.Parse(reader["id"].ToString() ?? string.Empty);
                    }
                }

                await connection.CloseAsync();
                        
            }
        }

        return result;
    }
    
    public async Task<int> SaveAsync(string name)
    {
        int? result = await GetIdByNameAsync(name);
        
        if (result is null)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"INSERT INTO {TableName}(`name`) VALUES(@name)";

                    command.Parameters.Clear();
                    command.Parameters.Add(new MySqlParameter("@name", name));

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    await connection.CloseAsync();

                }
            }

            result = await GetIdByNameAsync(name);
        }

        return result ?? 0;
    }

    public async Task<int?> GetConnectIdByDeviceIdAsync(int deviceId, int sensorId)
    {
        int? result = null;
        
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT `id` FROM {ConnectTableName} WHERE `device_id` = @device_id AND `sensor_id` = @sensor_id";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@device_id", deviceId));
                command.Parameters.Add(new MySqlParameter("@sensor_id", sensorId));

                await connection.OpenAsync();

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = int.Parse(reader["id"].ToString() ?? string.Empty);
                    }
                }

                await connection.CloseAsync();
                        
            }
        }

        return result;
    }
    
    public async Task<int> SaveConnectToDeviceAsync(int deviceId, int sensorId)
    {
        int? result = await GetConnectIdByDeviceIdAsync(deviceId, sensorId);

        if (result is null)
        {

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        $"INSERT INTO {ConnectTableName} (`device_id`, `sensor_id`) VALUES (@device_id, @sensor_id)";

                    command.Parameters.Clear();
                    command.Parameters.Add(new MySqlParameter("@device_id", deviceId));
                    command.Parameters.Add(new MySqlParameter("@sensor_id", sensorId));

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    await connection.CloseAsync();

                }
            }
            
            result = await GetConnectIdByDeviceIdAsync(deviceId, sensorId);
        }
        
        return result ?? 0;
    }
}