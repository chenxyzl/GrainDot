using System;
using System.Threading.Tasks;
using Base;
using Base.Network;

namespace Home.Model.Component;

public class WsComponent : IGlobalComponent
{
    public IWebSocketServer _server;
    public readonly ushort port;

    public WsComponent(ushort _port)
    {
        port = _port;
    }
}