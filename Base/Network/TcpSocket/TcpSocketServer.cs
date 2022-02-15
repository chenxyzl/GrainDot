using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Base.Network;

internal class TcpSocketServer<T> : BaseTcpSocketServer<ITcpSocketServer, ITcpSocketConnection, byte[]>,
    ITcpSocketServer
    where T : TcpSocketConnection
{
    public TcpSocketServer(int port,
        TcpSocketServerEvent<ITcpSocketServer, ITcpSocketConnection, byte[]> eventHandle)
        : base(port, eventHandle)
    {
    }

    public override void OnChannelReceive(IChannelHandlerContext ctx, object msg)
    {
        PackException(() =>
        {
            var bytes = (A.NotNull(msg as IByteBuffer)).ToArray();
            var theConnection = GetConnection(ctx.Channel);
            _eventHandle.OnRecieve?.Invoke(this, theConnection, bytes);
            theConnection.OnRecieve(bytes);
        });
    }

    protected override ITcpSocketConnection BuildConnection(IChannel clientChannel)
    {
        var arg = new object[] {this, clientChannel, _eventHandle};
        return A.NotNull(Activator.CreateInstance(typeof(T), arg) as T);
    }
}