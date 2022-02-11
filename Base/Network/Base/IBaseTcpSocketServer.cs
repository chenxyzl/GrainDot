using System.Collections.Generic;

namespace Base.Network;

/// <summary>
///     TcpSocketServer基接口
/// </summary>
/// <typeparam name="SocketConnection">The type of the ocket connection.</typeparam>
/// <seealso cref="Base.Network.IClose" />
public interface IBaseTcpSocketServer<SocketConnection> : IClose
    where SocketConnection : IBaseSocketConnection
{
    /// <summary>
    ///     监听端口
    /// </summary>
    /// <value>
    ///     The port.
    /// </value>
    int Port { get; }

    /// <summary>
    ///     通过连接Id获取连接
    /// </summary>
    /// <param name="connectionId">连接Id</param>
    /// <returns></returns>
    SocketConnection GetConnectionById(string connectionId);

    /// <summary>
    ///     删除连接
    /// </summary>
    /// <param name="theConnection">连接</param>
    void RemoveConnection(SocketConnection theConnection);

    /// <summary>
    ///     获取所有连接
    /// </summary>
    /// <returns></returns>
    List<SocketConnection> GetAllConnections();

    /// <summary>
    ///     获取当前连接数量
    /// </summary>
    /// <returns></returns>
    int GetConnectionCount();
}