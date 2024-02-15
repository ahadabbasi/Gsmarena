using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Gsmarena.WindowsApplication.Models.Repositories;

public class ApplicationContext
{
    public string ConnectionString { get; }

    public ApplicationContext(string connectionString)
    {
        ConnectionString = connectionString;
    }

    private BrandRepository? _brand;

    public BrandRepository Brand
    {
        get
        {
            if (_brand == null)
            {
                _brand = new BrandRepository(ConnectionString);
            }

            return _brand;
        }
    }
    
    private DeviceRepository? _device;

    public DeviceRepository Device
    {
        get
        {
            if (_device == null)
            {
                _device = new DeviceRepository(ConnectionString);
            }

            return _device;
        }
    }
    
    private NetworkRepository? _network;

    public NetworkRepository Network
    {
        get
        {
            if (_network == null)
            {
                _network = new NetworkRepository(ConnectionString);
            }

            return _network;
        }
    }
    
    private OperationSystemRepository? _operationSystem;

    public OperationSystemRepository OperationSystem
    {
        get
        {
            if (_operationSystem == null)
            {
                _operationSystem = new OperationSystemRepository(ConnectionString);
            }

            return _operationSystem;
        }
    }
    
    private CameraRepository? _camera;

    public CameraRepository Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = new CameraRepository(ConnectionString);
            }

            return _camera;
        }
    }
    
    private DimensionRepository? _dimension;

    public DimensionRepository Dimension
    {
        get
        {
            if (_dimension == null)
            {
                _dimension = new DimensionRepository(ConnectionString);
            }

            return _dimension;
        }
    }
    
    private TechnologyRepository? _technology;

    public TechnologyRepository Technology
    {
        get
        {
            if (_technology == null)
            {
                _technology = new TechnologyRepository(ConnectionString);
            }

            return _technology;
        }
    }
    
    private MemoryRepository? _memory;

    public MemoryRepository Memory
    {
        get
        {
            if (_memory == null)
            {
                _memory = new MemoryRepository(ConnectionString);
            }

            return _memory;
        }
    }
    
    private SensorRepository? _sensor;

    public SensorRepository Sensor
    {
        get
        {
            if (_sensor == null)
            {
                _sensor = new SensorRepository(ConnectionString);
            }

            return _sensor;
        }
    }

    public async Task CreateTable()
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText =
                    await File.ReadAllTextAsync(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tables.sql")
                    );

                await connection.OpenAsync();

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
        }
    }
}