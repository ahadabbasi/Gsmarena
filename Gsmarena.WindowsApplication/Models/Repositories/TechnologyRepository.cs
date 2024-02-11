using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.Repositories;

public class TechnologyRepository
{
    
    protected string ConnectionString { get; }

    protected string TableName
    {
        get
        {
            return "`technologies`";
        }
    }
    
    protected string ConnectTableName
    {
        get
        {
            return "`devices_technologies`";
        }
    }

    public TechnologyRepository(string connectionString)
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

    public async Task<int?> GetConnectIdByDeviceIdAsync(int deviceId, int technologyId)
    {
        int? result = null;
        
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT `id` FROM {ConnectTableName} WHERE `device_id` = @device_id AND `technology_id` = @technology_id";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@device_id", deviceId));
                command.Parameters.Add(new MySqlParameter("@technology_id", technologyId));

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
    
    public async Task<int> SaveConnectToDeviceAsync(int deviceId, int technologyId)
    {
        int? result = await GetConnectIdByDeviceIdAsync(deviceId, technologyId);

        if (result is null)
        {

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        $"INSERT INTO {ConnectTableName} (`device_id`, `technology_id`) VALUES (@device_id, @technology_id)";

                    command.Parameters.Clear();
                    command.Parameters.Add(new MySqlParameter("@device_id", deviceId));
                    command.Parameters.Add(new MySqlParameter("@technology_id", technologyId));

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    await connection.CloseAsync();

                }
            }
            
            result = await GetConnectIdByDeviceIdAsync(deviceId, technologyId);
        }
        
        return result ?? 0;
    }
}