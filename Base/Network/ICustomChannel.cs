using System.Threading.Tasks;
using Base.Helper;

namespace Base.Network;

public abstract class ICustomChannel
{
    protected IBaseSocketConnection _conn;
    public bool authed = false;

    public ICustomChannel(IBaseSocketConnection conn)
    {
        _conn = conn;
        ConnTime = TimeHelper.Now();
    }

    public long ConnTime { get; }

    public string ConnectionId => _conn.ConnectionId;

    public abstract void OnConnected();

    public abstract void OnRecieve(byte[] bytes);

    public Task Send(byte[] bytes, uint opcode)
    {
        if (bytes.Length >= ushort.MaxValue)
        {
            GlobalLog.Error($"channel:{ConnectionId} bytes too long,size:{bytes.Length} opcode:{opcode}");
            return Task.CompletedTask;
        }
        //这里没有必要等，要等的话还要切换线程恢复线程
        _ = _conn.Send(bytes);
        return Task.CompletedTask;
    }

    public virtual void Close()
    {
        _conn.Close();
    }
}