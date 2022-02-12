namespace Base.Network;

public abstract class ICustomChannel
{
    protected IBaseSocketConnection _conn;

    public ICustomChannel(IBaseSocketConnection conn)
    {
        _conn = conn;
    }

    public abstract void OnConnected();

    public abstract void OnRecieve(byte[] bytes);

    public virtual void Close()
    {
        _conn = null;
    }
}