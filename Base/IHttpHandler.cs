using System.Threading.Tasks;
using Base.Serialize;
using Message;

namespace Base;

public interface IHttpHandler
{
    Task<byte[]> Handle(byte[] data);
}

public abstract class HttpHandler<REQ, RSP> : IHttpHandler where REQ : class, IRequest where RSP : class, IResponse
{
    public async Task<byte[]> Handle(byte[] data)
    {
        var msg = SerializeHelper.FromBinary<RSP>(data);
        var ret = await Run(msg);
        return ret.ToBinary();
    }

    protected abstract Task<REQ> Run(RSP data);
}