using System;
using System.Data;
using System.Threading.Tasks;
using Gsmarena.WindowsApplication.Models.DataTransfers;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.Repositories;

public class MemoryRepository
{
    protected string ConnectionString { get; }

    protected string TableName
    {
        get
        {
            return "`unit_of_memories`";
        }
    }
    
    protected string ConnectTableName
    {
        get
        {
            return "`devices_memories`";
        }
    }

    public MemoryRepository(string connectionString)
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

    public async Task SaveAsync(MemoryDTO entry)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                int memoryUnitId = await GetIdByNameAsync(entry.MemoryUnit) ?? throw new Exception();
                int? ramUnitId = entry.RamUnit != null ? await GetIdByNameAsync(entry.RamUnit) : null;
                
                command.CommandType = CommandType.Text;
                command.CommandText = $"INSERT INTO {ConnectTableName} (`device_id`, `memory_size`, `memory_unit_id`, `ram_size`, `ram_unit_id`) VALUES (@device_id, @memory_size, @memory_unit_id, @ram_size, @ram_unit_id)";

                command.Parameters.Clear();
                command.Parameters.Add(new MySqlParameter("@device_id", entry.DeviceId));
                command.Parameters.Add(new MySqlParameter("@memory_size", entry.MemorySize));
                command.Parameters.Add(new MySqlParameter("@memory_unit_id", memoryUnitId));
                command.Parameters.Add(new MySqlParameter("@ram_size", entry.RamSize));
                command.Parameters.Add(new MySqlParameter("@ram_unit_id", ramUnitId));

                await connection.OpenAsync();

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
                        
            }
        }
    }
}