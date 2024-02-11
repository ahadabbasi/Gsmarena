using System.Data;
using System.Threading.Tasks;
using Gsmarena.WindowsApplication.Models.DataTransfers;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.Repositories;

public class DimensionRepository
{
    protected string ConnectionString { get; }

    protected string TableName
    {
        get
        {
            return "`networks`";
        }
    }

    public DimensionRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public async Task<int?> GetIdByDeviceIdAsync(int deviceId)
    {
        int? result = null;
        
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT `id` FROM {TableName} WHERE `device_id` = @device_id";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@device_id", deviceId));

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
    
    public async Task<int> SaveAsync(DimensionDTO entry)
    {
        int? result = await GetIdByDeviceIdAsync(entry.DeviceId);
        if (result is null)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        $"INSERT INTO {TableName}(`device_id`, `height`, `width`, `depth`) VALUES(@device_id, @height, @width, @depth)";

                    command.Parameters.Clear();
                    command.Parameters.Add(new MySqlParameter("@device_id", entry.DeviceId));
                    command.Parameters.Add(new MySqlParameter("@height", entry.Height));
                    command.Parameters.Add(new MySqlParameter("@width", entry.Width));
                    command.Parameters.Add(new MySqlParameter("@depth", entry.Depth));

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    await connection.CloseAsync();

                }
            }
        }

        result = await GetIdByDeviceIdAsync(entry.DeviceId);

        return result ?? 0;
    }

}