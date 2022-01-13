using DotNetty.Transport.Channels;
using System;

namespace Base.Network
{
    interface IChannelEvent
    {
        void OnChannelActive(IChannelHandlerContext ctx);
        void OnChannelReceive(IChannelHandlerContext ctx, object msg);
        void OnChannelInactive(IChannel channel);
        void OnException(IChannel channel, Exception exception);
    }
}