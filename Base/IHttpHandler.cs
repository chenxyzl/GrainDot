using System.Threading.Tasks;
using Base.Serializer;
using Message;

namespace Base
{
    public interface IHttpHandler
    {
        Task<byte[]> Handle(byte[] data);
    }

    public abstract class HttpHandler<R, T> : IHttpHandler where T : class, IRequest where R : class, IResponse
    {
        protected abstract Task<R> Run(T data);

        public async Task<byte[]> Handle(byte[] data)
        {
            var msg = SerializerHelper.FromBinary<T>(data);
            var ret = await Run(msg);
            return ret.ToBinary();
        }
    }
}