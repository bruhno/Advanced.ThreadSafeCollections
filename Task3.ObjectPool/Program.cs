using System.Collections.Concurrent;
using System.Data;

var random = new Random();

using var pool = new ConnectionPool();

Task.WaitAll(Enumerable.Range(1, 1000).Select(Process));

Console.WriteLine($"Pool Size {pool.Connections.Count()}");

foreach (var c in pool.Connections)
{
    Console.WriteLine($"{c.Number} use {c.UseCount}");
}

async Task Process(int num)
{
    var delay = random.Next(num * 10);

    await Task.Delay(delay);

    using var c = pool.GetConnection();

    c.SomeProcess();
}

public interface IConnection: IDisposable
{
    int UseCount { get; }

    public int Number { get; }

    public void SomeProcess();
}

public class ConnectionPool:IDisposable
{
    public IEnumerable<IConnection> Connections => _connections.ToList();

    public IConnection GetConnection()
    {
        if (disposedValue) throw new InvalidOperationException();

        if (!_connections.TryTake(out var cc))
        {
            cc = new CoreConnection();
        }

        return new Connection(this,cc);
    }

    public void Add(CoreConnection connection)
    {
        if (disposedValue) throw new InvalidOperationException();

        _connections.Add(connection);
    }

    private ConcurrentBag<CoreConnection> _connections = new();

    #region dispose pattern
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                foreach (var c in _connections)
                {
                    c.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~ConnectionPool()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}


public sealed class Connection: IConnection, IDisposable
{
    public CoreConnection CoreConnection { get; private set; }

    public int UseCount => CoreConnection.UseCount;

    public int Number => CoreConnection.Number;
    
    public Connection(ConnectionPool connectionPool, CoreConnection coreConnection)
    {
        _connectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
        CoreConnection = coreConnection ?? throw new ArgumentNullException(nameof(coreConnection));        
    }

    public void SomeProcess() => CoreConnection.SomeProcess();

    private void ReturnToPool()
    {
        _connectionPool.Add(CoreConnection);
    }

    private readonly ConnectionPool _connectionPool;

    #region dispose pattern
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                ReturnToPool();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Connection()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private bool _disposedValue;
    #endregion
}

public class CoreConnection:IConnection
{
    public int UseCount { get; private set; }

    public int Number { get; private set; }

    public CoreConnection()
    {
        Number = Interlocked.Increment(ref _numberSequence);
    }

    public void SomeProcess()
    {
        if (disposedValue) throw new InvalidOperationException();

        UseCount++;
    }

    private static int _numberSequence = 0;

    #region dispose pattern
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~CoreConnection()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}