using System.Data;
using System.Threading.Tasks;
using Gsmarena.WindowsApplication.Models.DataTransfers;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.Repositories;

public class DeviceRepository
{
    protected string ConnectionString { get; }

    protected string TableName
    {
        get { return "`devices`"; }
    }

    public DeviceRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public async Task<int?> GetIdByUrl(string url)
    {
        int? result = null;

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT `id` FROM {TableName} WHERE `url` = @url";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@url", url));

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

    public async Task<int> SaveAsync(DeviceDTO entry)
    {
        int? id = await GetIdByUrl(entry.Url);

        if (id is null)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        $"INSERT INTO {TableName}(`url`, `name`, `brand_id`, `type`, `display_ratio`, `display_size`, `weight`, `battery_capacity`, `pixel_per_inches`, `processor_model`, `count_of_thread`, `price`, `year_of_release`, `operation_id`, `operation_system_version`) VALUES (@url, @name, @brand_id, @type, @display_ratio, @display_size, @weight, @battery_capacity, @pixel_per_inches, @processor_model, @count_of_thread, @price, @year_of_release, @operation_id, @operation_system_version)";

                    command.Parameters.Clear();
                    command.Parameters.Add(new MySqlParameter("@url", entry.Url));
                    command.Parameters.Add(new MySqlParameter("@name", entry.Name));
                    command.Parameters.Add(new MySqlParameter("@brand_id", entry.BrandId));
                    command.Parameters.Add(new MySqlParameter("@type", entry.Type.ToUpper()));
                    command.Parameters.Add(new MySqlParameter("@display_ratio", entry.DisplayRatio));
                    command.Parameters.Add(new MySqlParameter("@display_size", entry.DisplaySize));
                    command.Parameters.Add(new MySqlParameter("@weight", entry.Weight));
                    command.Parameters.Add(new MySqlParameter("@battery_capacity", entry.BatteryCapacity));
                    command.Parameters.Add(new MySqlParameter("@pixel_per_inches", entry.PixelPerInch));
                    command.Parameters.Add(new MySqlParameter("@processor_model", entry.CpuModel));
                    command.Parameters.Add(new MySqlParameter("@count_of_thread", entry.CountOfThread));
                    command.Parameters.Add(new MySqlParameter("@price", entry.Price));
                    command.Parameters.Add(new MySqlParameter("@year_of_release",
                        entry.ReleaseDate.ToString("yyyy-M-d")));
                    command.Parameters.Add(new MySqlParameter("@operation_id", entry.OperationSystemId));
                    command.Parameters.Add(new MySqlParameter("@operation_system_version", entry.OperationSystemVersion));

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();

                    await connection.CloseAsync();
                }
            }
        }

        id = await GetIdByUrl(entry.Url);

        return id ?? 0;
    }


    public async Task UpdateAsync(DeviceDTO entry)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText =
                    $"UPDATE {TableName} SET `name` = @name, `brand_id` = @brand_id, `type` = @type, `display_ratio` = @display_ratio, `display_size` = @display_size, `weight` = @weight, `battery_capacity` = @battery_capacity, `pixel_per_inches` = @pixel_per_inches, `processor_model` = @processor_model, `count_of_thread` = @count_of_thread, `price` = @price, `year_of_release` = @year_of_release, `operation_id` = @operation_id, `operation_system_version` = @operation_system_version WHERE `url` = @url";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@url", entry.Url));
                command.Parameters.Add(new MySqlParameter("@name", entry.Name));
                command.Parameters.Add(new MySqlParameter("@brand_id", entry.BrandId));
                command.Parameters.Add(new MySqlParameter("@type", entry.Type.ToUpper()));
                command.Parameters.Add(new MySqlParameter("@display_ratio", entry.DisplayRatio));
                command.Parameters.Add(new MySqlParameter("@display_size", entry.DisplaySize));
                command.Parameters.Add(new MySqlParameter("@weight", entry.Weight));
                command.Parameters.Add(new MySqlParameter("@battery_capacity", entry.BatteryCapacity));
                command.Parameters.Add(new MySqlParameter("@pixel_per_inches", entry.PixelPerInch));
                command.Parameters.Add(new MySqlParameter("@processor_model", entry.CpuModel));
                command.Parameters.Add(new MySqlParameter("@count_of_thread", entry.CountOfThread));
                command.Parameters.Add(new MySqlParameter("@price", entry.Price));
                command.Parameters.Add(new MySqlParameter("@year_of_release",
                    entry.ReleaseDate.ToString("yyyy-M-d")));
                command.Parameters.Add(new MySqlParameter("@operation_id", entry.OperationSystemId));
                command.Parameters.Add(new MySqlParameter("@operation_system_version", entry.OperationSystemVersion));

                await connection.OpenAsync();

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
        }
    }
}