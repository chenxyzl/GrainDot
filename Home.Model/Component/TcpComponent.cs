using System.Net;
using Base;
using Base.Network;

namespace Home.Model.Component;

public class TcpComponent : IGlobalComponent
{
    public readonly IPAddress ip;
    public readonly ushort port;
    public ITcpSocketServer _server = null!;

    public TcpComponent(IPAddress _ip, ushort _port)
    {
        ip = _ip;
        port = _port;
    }
}