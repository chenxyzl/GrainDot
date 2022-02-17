using System.Net;

namespace Base.Network;

internal abstract class BaseGenericClientBuilder<TBuilder, TTarget, TData> :
    BaseBuilder<TBuilder, TTarget>,
    IGenericClientBuilder<TBuilder, TTarget, TData>
    where TBuilder : class
{
    public BaseGenericClientBuilder(IPAddress ip, int port)
        : base(ip, port)
    {
    }

    protected TcpSocketCientEvent<TTarget, TData> _event { get; } = new();

    public TBuilder OnClientClose(Action<TTarget> action)
    {
        _event.OnClientClose = action;

        return A.NotNull(this as TBuilder);
    }

    public TBuilder OnClientStarted(Action<TTarget> action)
    {
        _event.OnClientStarted = action;

        return A.NotNull(this as TBuilder);
    }

    public TBuilder OnRecieve(Action<TTarget, TData> action)
    {
        _event.OnRecieve = action;

        return A.NotNull(this as TBuilder);
    }

    public TBuilder OnSend(Action<TTarget, TData> action)
    {
        _event.OnSend = action;

        return A.NotNull(this as TBuilder);
    }

    public override TBuilder OnException(Action<Exception> action)
    {
        _event.OnException = action;

        return A.NotNull(this as TBuilder);
    }
}