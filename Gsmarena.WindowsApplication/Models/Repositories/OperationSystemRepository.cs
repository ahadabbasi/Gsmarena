﻿using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.Repositories;

public class OperationSystemRepository
{
    protected string ConnectionString { get; }
    protected string TableName
    {
        get { return "`operation_systems`"; }
    }

    public OperationSystemRepository(string connectionString)
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
}