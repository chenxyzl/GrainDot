namespace Base.Network
{
    /// <summary>
    /// WebSocket服务端构建者
    /// </summary>
    public interface IWebSocketServerBuilder : IGenericServerBuilder<IWebSocketServerBuilder, IWebSocketServer, IWebSocketConnection, byte[]>
    {

    }
}
