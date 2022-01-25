namespace Base.Network;

/// <summary>
///     Tcp和Udp的事件相关
/// </summary>
/// <typeparam name="TSocketServer"></typeparam>
/// <typeparam name="TConnection"></typeparam>
/// <typeparam name="TData"></typeparam>
public class TcpSocketServerEvent<TSocketServer, TConnection, TData>
{
    /// <summary>
    ///     服务启动完成
    /// </summary>
    public Action<TSocketServer> OnServerStarted { get; set; }

    /// <summary>
    ///     有新链接接入
    /// </summary>
    public Action<TSocketServer, TConnection> OnNewConnection { get; set; }

    /// <summary>
    ///     接收到新数据
    /// </summary>
    public Action<TSocketServer, TConnection, TData> OnRecieve { get; set; }

    /// <summary>
    ///     链接关闭
    /// </summary>
    public Action<TSocketServer, TConnection> OnConnectionClose { get; set; }

    /// <summary>
    ///     socket服务器出现异常
    /// </summary>
    public Action<Exception> OnException { get; set; }
}