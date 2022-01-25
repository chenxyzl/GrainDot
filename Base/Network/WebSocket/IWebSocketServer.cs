namespace Base.Network;

/// <summary>
///     WebSocket服务端
/// </summary>
public interface IWebSocketServer : IBaseTcpSocketServer<IWebSocketConnection>
{
}