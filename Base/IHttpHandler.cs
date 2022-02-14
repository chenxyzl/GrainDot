using System.Threading.Tasks;
using Base.Serialize;
using Message;

namespace Base;

public interface IHttpHandler
{
    Task<byte[]> Handle(byte[] data);
}

public abstract class HttpHandler<REQ, RSP> : IHttpHandler
    where REQ : class, IHttpRequest where RSP : class, IHttpResponse
{
    public async Task<byte[]> Handle(byte[] data)
    {
        var msg = SerializeHelper.FromBinary<REQ>(data);
        var ret = await Run(msg);
        return ret.ToBinary();
    }

    protected abstract Task<RSP> Run(REQ data);
}