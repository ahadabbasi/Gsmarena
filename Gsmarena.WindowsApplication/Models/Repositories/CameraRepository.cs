using System.Data;
using System.Threading.Tasks;
using Gsmarena.WindowsApplication.Models.DataTransfers;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.Repositories;

public class CameraRepository
{
    protected string ConnectionString { get; }

    protected string TableName
    {
        get { return "`devices_cameras`"; }
    }

    public CameraRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public async Task<int?> GetIdByDeviceAsync(CameraDTO entry)
    {
        int? result = null;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText =
                    $"SELECT `id` FROM {TableName} WHERE `device_id` = @device_id AND `pixel` = @pixel AND `position` = @position";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@device_id", entry.DeviceId));
                command.Parameters.Add(new MySqlParameter("@pixel", entry.Pixel));
                command.Parameters.Add(new MySqlParameter("@position", entry.Position));

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

    public async Task SaveAsync(CameraDTO entry)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText =
                    $"INSERT INTO {TableName}(`device_id`, `pixel`, `position`, `type`) VALUES(@device_id, @pixel, @position, @type)";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@device_id", entry.DeviceId));
                command.Parameters.Add(new MySqlParameter("@pixel", entry.Pixel));
                command.Parameters.Add(new MySqlParameter("@position", entry.Position));
                command.Parameters.Add(new MySqlParameter("@type", entry.Type));

                await connection.OpenAsync();

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
        }
    }
}