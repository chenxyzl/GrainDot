using Base;
using Base.Network;

namespace Home.Model.Component;

public class TcpComponent : IGlobalComponent
{
    public readonly ushort port;
    public ITcpSocketServer _server = null!;

    public TcpComponent(ushort _port)
    {
        port = _port;
    }
}