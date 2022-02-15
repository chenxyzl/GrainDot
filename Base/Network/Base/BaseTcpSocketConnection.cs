using System.Net;
using System.Threading.Tasks;
using Base.Helper;
using DotNetty.Transport.Channels;

namespace Base.Network;

/// <summary>
///     tcp和websocket链接的基础类
/// </summary>
/// <typeparam name="TTcpSocketServer"></typeparam>
/// <typeparam name="TConnection"></typeparam>
/// <typeparam name="TData"></typeparam>
public abstract class BaseTcpSocketConnection<TTcpSocketServer, TConnection, TData>
    : IBaseSocketConnection
    where TConnection : class, IBaseSocketConnection
    where TTcpSocketServer : IBaseTcpSocketServer<TConnection>
{
    #region 构造函数

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="server"></param>
    /// <param name="channel"></param>
    /// <param name="serverEvent"></param>
    public BaseTcpSocketConnection(
        TTcpSocketServer server,
        IChannel channel,
        TcpSocketServerEvent<TTcpSocketServer, TConnection, TData> serverEvent)
    {
        _server = server;
        _channel = channel;
        _serverEvent = serverEvent;
        time = TimeHelper.Now();
    }

    #endregion

    #region 私有成员

    /// <summary>
    ///     tcp server
    /// </summary>
    protected TTcpSocketServer _server { get; }

    /// <summary>
    ///     channel
    /// </summary>
    protected IChannel _channel { get; }

    /// <summary>
    ///     链接事件
    /// </summary>
    protected TcpSocketServerEvent<TTcpSocketServer, TConnection, TData> _serverEvent { get; }

    #endregion

    #region 外部接口

    /// <summary>
    ///     链接id
    /// </summary>
    public string ConnectionId => _channel.Id.AsShortText();

    /// <summary>
    ///     客户端地址
    /// </summary>
    public IPEndPoint ClientAddress => A.NotNull(_channel.RemoteAddress as IPEndPoint);

    public long time { get; }

    /// <summary>
    ///     关闭链接
    /// </summary>
    public void Close()
    {
        _channel.CloseAsync();
    }

    #endregion

    #region 继承的事件相关

    /// <summary>
    ///     新连接
    /// </summary>
    public abstract void OnConnected();

    /// <summary>
    /// </summary>
    public abstract void OnRecieve(byte[] bytes);

    /// <summary>
    ///     链接关闭
    /// </summary>
    public abstract void OnClose();

    /// <summary>
    ///     发送byes[]
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public abstract Task Send(byte[] bytes);

    /// <summary>
    ///     发送string
    /// </summary>
    /// <param name="msgStr"></param>
    /// <returns></returns>
    public abstract Task Send(string msgStr);

    #endregion
}