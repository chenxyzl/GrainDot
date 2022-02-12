using System.Threading.Tasks;

namespace Base.Network;

public abstract class ICustomChannel
{
    protected IBaseSocketConnection _conn;
    public bool authed = false;
    public string ConnectionId => _conn.ConnectionId;

    public ICustomChannel(IBaseSocketConnection conn)
    {
        _conn = conn;
    }

    public abstract void OnConnected();

    public abstract void OnRecieve(byte[] bytes);

    public async Task Send(byte[] bytes)
    {
        await _conn.Send(bytes);
    }

    public virtual void Close()
    {
        _conn = null;
    }
}