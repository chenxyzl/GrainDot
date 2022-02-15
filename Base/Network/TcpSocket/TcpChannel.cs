using DotNetty.Transport.Channels;

namespace Base.Network;

public class TcpChannel<T> : TcpSocketConnection where T : ICustomChannel
{
    private readonly T _customChannel;

    public TcpChannel(ITcpSocketServer server, IChannel channel,
        TcpSocketServerEvent<ITcpSocketServer, ITcpSocketConnection, byte[]> serverEvent) : base(server, channel,
        serverEvent)
    {
        _customChannel = A.NotNull(Activator.CreateInstance(typeof(T), this) as T);
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