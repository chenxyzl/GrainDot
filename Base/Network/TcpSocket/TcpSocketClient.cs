using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Base.Network;

internal class TcpSocketClient : BaseSocketClient<ISocketClient, byte[]>, ISocketClient
{
    public TcpSocketClient(string ip, int port, TcpSocketCientEvent<ISocketClient, byte[]> clientEvent)
        : base(ip, port, clientEvent)
    {
    }

    public override async Task Send(byte[] bytes)
    {
        try
        {
            await _channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
            await Task.Run(() => { _clientEvent.OnSend?.Invoke(this, bytes); });
        }
        catch (Exception ex)
        {
            _clientEvent.OnException?.Invoke(ex);
        }
    }

    public override async Task Send(string msgStr)
    {
        await Send(Encoding.UTF8.GetBytes(msgStr));
    }

    public override void OnChannelReceive(IChannelHandlerContext ctx, object msg)
    {
        PackException(() =>
        {
            var bytes = (A.NotNull(msg as IByteBuffer)).ToArray();
            _clientEvent.OnRecieve?.Invoke(this, bytes);
        });
    }
}