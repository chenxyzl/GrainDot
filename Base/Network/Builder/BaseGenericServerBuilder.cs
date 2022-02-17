using System.Net;

namespace Base.Network;

internal abstract class BaseGenericServerBuilder<TBuilder, TTarget, IConnection, TData> :
    BaseBuilder<TBuilder, TTarget>,
    IGenericServerBuilder<TBuilder, TTarget, IConnection, TData>
    where TBuilder : class
{
    public BaseGenericServerBuilder(IPAddress ip, int port)
        : base(ip, port)
    {
    }

    protected TcpSocketServerEvent<TTarget, IConnection, TData> _event { get; } = new();

    public TBuilder OnConnectionClose(Action<TTarget, IConnection> action)
    {
        _event.OnConnectionClose = action;

        return A.NotNull(this as TBuilder);
    }

    public TBuilder OnNewConnection(Action<TTarget, IConnection> action)
    {
        _event.OnNewConnection = action;

        return A.NotNull(this as TBuilder);
    }

    public TBuilder OnServerStarted(Action<TTarget> action)
    {
        _event.OnServerStarted = action;

        return A.NotNull(this as TBuilder);
    }

    public override TBuilder OnException(Action<Exception> action)
    {
        _event.OnException = action;

        return A.NotNull(this as TBuilder);
    }

    public TBuilder OnRecieve(Action<TTarget, IConnection, TData> action)
    {
        _event.OnRecieve = action;

        return A.NotNull(this as TBuilder);
    }

    public TBuilder OnSend(Action<TTarget, IConnection, TData> action)
    {
        return A.NotNull(this as TBuilder);
    }
}