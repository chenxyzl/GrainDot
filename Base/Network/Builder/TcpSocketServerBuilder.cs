﻿using System.Net;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Base.Network;

internal class TcpSocketServerBuilder<T> :
    BaseGenericServerBuilder<ITcpSocketServerBuilder, ITcpSocketServer, ITcpSocketConnection, byte[]>,
    ITcpSocketServerBuilder where T : TcpSocketConnection
{
    public TcpSocketServerBuilder(IPAddress ip, int port)
        : base(ip, port)
    {
    }

    protected Action<IChannelPipeline>? _setEncoder { get; set; }

    public ITcpSocketServerBuilder SetLengthFieldDecoder(int maxFrameLength, int lengthFieldOffset,
        int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip,
        ByteOrder byteOrder = ByteOrder.BigEndian)
    {
        _setEncoder += x => x.AddLast(new LengthFieldBasedFrameDecoder(byteOrder, maxFrameLength, lengthFieldOffset,
            lengthFieldLength, lengthAdjustment, initialBytesToStrip, true));

        return this;
    }

    public ITcpSocketServerBuilder SetLengthFieldEncoder(int lengthFieldLength)
    {
        _setEncoder += x => x.AddLast(new LengthFieldPrepender(lengthFieldLength));

        return this;
    }

    public override async Task<ITcpSocketServer> BuildAsync()
    {
        var tcpServer = new TcpSocketServer<T>(_ip, _port, _event);

        var serverChannel = await new ServerBootstrap()
            .Group(new MultithreadEventLoopGroup(), new MultithreadEventLoopGroup())
            .Channel<TcpServerSocketChannel>()
            .Option(ChannelOption.SoBacklog, 1024)
            .Option(ChannelOption.TcpNodelay, true)
            .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                var pipeline = channel.Pipeline;
                _setEncoder?.Invoke(pipeline);
                pipeline.AddLast(new CommonChannelHandler(tcpServer));
            })).BindAsync(_ip, _port);
        _event.OnServerStarted?.Invoke(tcpServer);
        tcpServer.SetChannel(serverChannel);

        return await Task.FromResult(tcpServer);
    }
}