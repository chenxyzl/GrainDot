using System.Threading.Tasks;
using Base.Serializer;
using Message;

namespace Base
{
    public interface IHttpHandler
    {
        Task<byte[]> Handle(byte[] data);
    }

    public abstract class HttpHandler<REQ, RSP> : IHttpHandler where REQ : class, IRequest where RSP: class, IResponse
    {
        protected abstract Task<REQ> Run(RSP data);

        public async Task<byte[]> Handle(byte[] data)
        {
            var msg = SerializerHelper.FromBinary<RSP>(data);
            var ret = await Run(msg);
            return ret.ToBinary();
        }
    }
}