using System.Net;
using System.Threading.Tasks;

namespace Base.Network;

internal abstract class BaseBuilder<TBuilder, TTarget> : IBuilder<TBuilder, TTarget> where TBuilder : class
{
    public BaseBuilder(IPAddress ip, int port)
    {
        _port = port;
        _ip = ip;
    }

    protected int _port { get; }
    protected IPAddress _ip { get; }

    public abstract Task<TTarget> BuildAsync();

    public abstract TBuilder OnException(Action<Exception> action);
}