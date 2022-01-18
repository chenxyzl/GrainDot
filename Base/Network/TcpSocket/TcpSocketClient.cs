using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Base.Network
{
    class TcpSocketClient : BaseSocketClient<ISocketClient, byte[]>, ISocketClient
    {
        public TcpSocketClient(string ip, int port, TcpSocketCientEvent<ISocketClient, byte[]> clientEvent)
            : base(ip, port, clientEvent)
        {
        }

        public override void OnChannelReceive(IChannelHandlerContext ctx, object msg)
        {
            PackException(() =>
            {
                var bytes = (msg as IByteBuffer).ToArray();
                _clientEvent.OnRecieve?.Invoke(this, bytes);
            });
        }

        public async override Task Send(byte[] bytes)
        {
            try
            {
                await _channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
                await Task.Run(() =>
                {
                    _clientEvent.OnSend?.Invoke(this, bytes);
                });
            }
            catch (Exception ex)
            {
                _clientEvent.OnException?.Invoke(ex);
            }
        }

        public async override Task Send(string msgStr)
        {
            await Send(Encoding.UTF8.GetBytes(msgStr));
        }
    }
}
