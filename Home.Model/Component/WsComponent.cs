using System.Net;
using Base;
using Base.Network;

namespace Home.Model.Component;

public class WsComponent : IGlobalComponent
{
    public readonly IPAddress ip;
    public readonly ushort port;
    public IWebSocketServer _server = null!;

    public WsComponent(IPAddress _ip, ushort _port)
    {
        ip = _ip;
        port = _port;
    }
}