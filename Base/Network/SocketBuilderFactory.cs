namespace Base.Network
{
    /// <summary>
    /// Socket构建者工厂
    /// </summary>
    public class SocketBuilderFactory
    {
        /// <summary>
        /// 获取TcpSocket客户端构建者
        /// </summary>
        /// <param name="ip">服务器Ip</param>
        /// <param name="port">服务器端口</param>
        /// <returns></returns>
        public static ITcpSocketClientBuilder GetTcpSocketClientBuilder(string ip, int port)
        {
            return new TcpSocketClientBuilder(ip, port);
        }

        /// <summary>
        /// 获取TcpSocket服务端构建者
        /// </summary>
        /// <param name="port">监听端口</param>
        /// <returns></returns>
        public static ITcpSocketServerBuilder GetTcpSocketServerBuilder<T>(int port) where T : TcpSocketConnection, new()
        {
            return new TcpSocketServerBuilder<T>(port);
        }

        /// <summary>
        /// 获取WebSocket服务端构建者
        /// </summary>
        /// <param name="port">监听端口</param>
        /// <param name="path">路径,默认为"/"</param>
        /// <returns></returns>
        public static IWebSocketServerBuilder GetWebSocketServerBuilder<T>(int port, string path = "/") where T : WebSocketConnection, new()
        {
            return new WebSocketServerBuilder<T>(port, path);
        }

        /// <summary>
        /// 获取WebSocket客户端构建者
        /// </summary>
        /// <param name="ip">服务器Ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="path">路径,默认为"/"</param>
        /// <returns></returns>
        public static IWebSocketClientBuilder GetWebSocketClientBuilder(string ip, int port, string path = "/")
        {
            return new WebSocketClientBuilder(ip, port, path);
        }
    }
}
