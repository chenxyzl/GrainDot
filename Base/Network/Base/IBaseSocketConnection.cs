using System.Net;

namespace Base.Network
{
    /// <summary>
    /// SocketConnection基接口
    /// </summary>
    /// <seealso cref="Base.Network.IClose" />
    public interface IBaseSocketConnection : IClose, ISendBytes, ISendString
    {
        /// <summary>
        /// 连接Id,不可更改
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        string ConnectionId { get; }

        /// <summary>
        /// 连接名,可更改
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        string ConnectionName { get; set; }

        /// <summary>
        /// 客户端地址
        /// </summary>
        /// <value>
        /// The client address.
        /// </value>
        IPEndPoint ClientAddress { get; }

        /// <summary>
        /// 新连接
        /// </summary>
        void OnConnected();

        /// <summary>
        /// 
        /// </summary>
        void OnRecieve(byte[] bytes);

        /// <summary>
        /// 链接关闭
        /// </summary>
        void OnClose();
    }
}
