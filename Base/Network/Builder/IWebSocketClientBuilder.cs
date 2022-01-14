namespace Base.Network
{
    /// <summary>
    /// WebSocket客户端构建者
    /// </summary>
    public interface IWebSocketClientBuilder :
        IGenericClientBuilder<IWebSocketClientBuilder, ISocketClient, byte[]>
    {

    }
}
