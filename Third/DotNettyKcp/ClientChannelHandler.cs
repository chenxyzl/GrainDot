using System;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace dotNetty_kcp;

public class ClientChannelHandler : ChannelHandlerAdapter
{
    private readonly ChannelConfig _channelConfig;
    private readonly IChannelManager _channelManager;


    public ClientChannelHandler(IChannelManager channelManager, ChannelConfig channelConfig)
    {
        _channelManager = channelManager;
        _channelConfig = channelConfig;
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        Console.WriteLine(exception);
    }

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        var msg = (DatagramPacket) message;
        var ukcp = _channelManager.get(msg);
        ukcp.read(msg.Content);
    }
}