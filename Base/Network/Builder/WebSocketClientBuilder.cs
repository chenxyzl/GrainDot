using System.Threading.Tasks;
using DotNetty.Codecs.Http;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Base.Network;

internal class WebSocketClientBuilder : BaseGenericClientBuilder<IWebSocketClientBuilder, ISocketClient, byte[]>,
    IWebSocketClientBuilder
{
    public WebSocketClientBuilder(string ip, int port, string path)
        : base(ip, port)
    {
        _path = path;
    }

    private string _path { get; }

    public override async Task<ISocketClient> BuildAsync()
    {
        var tcpClient = new WebSocketClient(_ip, _port, _path, _event);

        var clientChannel = await new Bootstrap()
            .Group(new MultithreadEventLoopGroup())
            .Channel<TcpSocketChannel>()
            .Option(ChannelOption.TcpNodelay, true)
            .Handler(new ActionChannelInitializer<IChannel>(channel =>
            {
                var pipeline = channel.Pipeline;
                pipeline.AddLast(
                    new HttpClientCodec(),
                    new HttpObjectAggregator(8192),
                    new CommonChannelHandler(tcpClient));
            })).ConnectAsync($"{_ip}:{_port}".ToIPEndPoint());
        await tcpClient.HandshakeCompletion;
        tcpClient.SetChannel(clientChannel);

        return await Task.FromResult(tcpClient);
    }
}