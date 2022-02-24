using System.Net;

namespace Base.Network;

/// <summary>
///     Socket构建者工厂
/// </summary>
public class SocketBuilderFactory
{
    /// <summary>
    ///     获取TcpSocket客户端构建者
    /// </summary>
    /// <param name="ip">服务器Ip</param>
    /// <param name="port">服务器端口</param>
    /// <returns></returns>
    public static ITcpSocketClientBuilder GetTcpSocketClientBuilder(IPAddress ip, int port)
    {
        return new TcpSocketClientBuilder(ip, port);
    }

    /// <summary>
    ///     获取TcpSocket服务端构建者
    /// </summary>
    /// <param name="port">监听端口</param>
    /// <returns></returns>
    public static ITcpSocketServerBuilder GetTcpSocketServerBuilder<T>(IPAddress ip, int port)
        where T : TcpSocketConnection
    {
        return new TcpSocketServerBuilder<T>(ip, port);
    }

    /// <summary>
    ///     获取WebSocket服务端构建者
    /// </summary>
    /// <param name="port">监听端口</param>
    /// <param name="path">路径,默认为"/"</param>
    /// <returns></returns>
    public static IWebSocketServerBuilder GetWebSocketServerBuilder<T>(IPAddress ip, int port, string path = "/")
        where T : WebSocketConnection
    {
        return new WebSocketServerBuilder<T>(ip, port, path);
    }

    /// <summary>
    ///     获取WebSocket客户端构建者
    /// </summary>
    /// <param name="ip">服务器Ip</param>
    /// <param name="port">服务器端口</param>
    /// <param name="path">路径,默认为"/"</param>
    /// <returns></returns>
    public static IWebSocketClientBuilder GetWebSocketClientBuilder(IPAddress ip, int port, string path = "/")
    {
        return new WebSocketClientBuilder(ip, port, path);
    }
}