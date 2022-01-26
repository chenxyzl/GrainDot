using System.Net;
using DotNetty.Transport.Channels;

namespace dotNetty_kcp;

public class User
{
    public User(IChannel channel, EndPoint remoteAddress, EndPoint localAddress)
    {
        Channel = channel;
        RemoteAddress = remoteAddress;
        LocalAddress = localAddress;
    }


    public IChannel Channel { get; set; }


    public EndPoint RemoteAddress { get; set; }

    public EndPoint LocalAddress { get; set; }

    public object O { get; set; }
}