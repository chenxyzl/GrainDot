using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Base.Network;

/// <summary>
///     tcp链接的虚类
/// </summary>
public abstract class TcpSocketConnection : BaseTcpSocketConnection<ITcpSocketServer, ITcpSocketConnection, byte[]>,
    ITcpSocketConnection
{
    #region 构造函数

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="server"></param>
    /// <param name="channel"></param>
    /// <param name="serverEvent"></param>
    public TcpSocketConnection(ITcpSocketServer server, IChannel channel,
        TcpSocketServerEvent<ITcpSocketServer, ITcpSocketConnection, byte[]> serverEvent)
        : base(server, channel, serverEvent)
    {
    }

    #endregion

    #region 私有成员

    #endregion

    #region 外部接口

    /// <summary>
    ///     发送 byte[]
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public override async Task Send(byte[] bytes)
    {
        try
        {
            await _channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
        }
        catch (Exception ex)
        {
            _serverEvent.OnException?.Invoke(ex);
        }
    }

    /// <summary>
    ///     发送 string
    /// </summary>
    /// <param name="msgStr"></param>
    /// <returns></returns>
    public override async Task Send(string msgStr)
    {
        await Send(Encoding.UTF8.GetBytes(msgStr));
    }

    #endregion
}