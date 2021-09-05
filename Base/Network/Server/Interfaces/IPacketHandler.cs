using Base.Network.Shared.Interfaces;

namespace Base.Network.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the server packet handler.
    /// </summary>
	internal interface IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from server packet handler.
        /// </summary>
        /// <param name="client">The connected client.</param>
        /// <param name="reader">The buffer reader received from message.</param>
        void HandleMessageData(IClient client, IBufferReader reader);
    }
}
