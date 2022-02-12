using DotNetty.Transport.Channels;

namespace Base.Network;

public class WebSocketChannel<T> : WebSocketConnection where T : ICustomChannel
{
    private readonly T _customChannel;

    public WebSocketChannel(IWebSocketServer server, IChannel channel,
        TcpSocketServerEvent<IWebSocketServer, IWebSocketConnection, byte[]> serverEvent) : base(server, channel,
        serverEvent)
    {
        _customChannel = Activator.CreateInstance(typeof(T), this) as T;
    }

    public override void OnClose()
    {
        _customChannel.Close();
    }

    public override void OnConnected()
    {
        _customChannel.OnConnected();
    }

    public override void OnRecieve(byte[] bytes)
    {
        _customChannel.OnRecieve(bytes);
    }
}