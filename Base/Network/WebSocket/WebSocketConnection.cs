using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;

namespace Base.Network;

/// <summary>
///     websocket链接的虚类
/// </summary>
public abstract class WebSocketConnection : BaseTcpSocketConnection<IWebSocketServer, IWebSocketConnection, byte[]>,
    IWebSocketConnection
{
    #region 构造函数

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="server"></param>
    /// <param name="channel"></param>
    /// <param name="serverEvent"></param>
    public WebSocketConnection(IWebSocketServer server, IChannel channel,
        TcpSocketServerEvent<IWebSocketServer, IWebSocketConnection, byte[]> serverEvent)
        : base(server, channel, serverEvent)
    {
    }

    #endregion

    #region 私有成员

    #endregion

    #region 外部接口

    /// <summary>
    ///     发送string
    /// </summary>
    /// <param name="msgStr"></param>
    /// <returns></returns>
    public override async Task Send(string msgStr)
    {
        try
        {
            await _channel.WriteAndFlushAsync(new TextWebSocketFrame(msgStr));
        }
        catch (Exception ex)
        {
            _serverEvent.OnException?.Invoke(ex);
        }
    }

    /// <summary>
    ///     发送二进制
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public override async Task Send(byte[] bytes)
    {
        try
        {
            await _channel.WriteAndFlushAsync(new BinaryWebSocketFrame(Unpooled.WrappedBuffer(bytes)));
        }
        catch (Exception ex)
        {
            _serverEvent.OnException?.Invoke(ex);
        }
    }

    #endregion
}