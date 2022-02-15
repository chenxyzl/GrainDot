using Base;
using Base.Network;

namespace Home.Model.Component;

public class WsComponent : IGlobalComponent
{
    public readonly ushort port;
    public IWebSocketServer _server = null!;

    public WsComponent(ushort _port)
    {
        port = _port;
    }
}