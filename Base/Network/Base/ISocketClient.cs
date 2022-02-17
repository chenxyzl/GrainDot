using System.Net;

namespace Base.Network;

/// <summary>
///     TcpSocket客户端
/// </summary>
/// <seealso cref="Base.Network.IClose" />
public interface ISocketClient : IClose, ISendBytes, ISendString
{
    /// <summary>
    ///     服务器Ip
    /// </summary>
    /// <value>
    ///     The ip.
    /// </value>
    IPAddress Ip { get; }

    /// <summary>
    ///     服务器端口
    /// </summary>
    /// <value>
    ///     The port.
    /// </value>
    int Port { get; }
}