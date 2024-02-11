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
}