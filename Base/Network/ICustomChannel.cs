using System.Threading.Tasks;
using Base.Helper;

namespace Base.Network;

public abstract class ICustomChannel
{
    protected IBaseSocketConnection _conn;
    public bool authed = false;
    public long ConnTime { get; private set; }

    public ICustomChannel(IBaseSocketConnection conn)
    {
        _conn = conn;
        ConnTime = TimeHelper.Now();
    }

    public string ConnectionId => _conn.ConnectionId;

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