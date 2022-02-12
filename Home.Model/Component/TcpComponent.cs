using System;
using System.Threading.Tasks;
using Base;
using Base.Network;

namespace Home.Model.Component;

public class TcpComponent : IGlobalComponent
{
    public ITcpSocketServer _server;

    public readonly ushort port;

    public TcpComponent(ushort _port)
    {
        port = _port;
    }
}