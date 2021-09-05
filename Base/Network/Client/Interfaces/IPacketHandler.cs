using Base.Network.Shared;
using Base.Network.Shared.Interfaces;

namespace Base.Network.Client.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client packet handler.
    /// </summary>
    internal interface IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from client packet handler.
        /// </summary>
        /// <param name="reader">The buffer reader of received message.</param>
        void HandleMessageData(IBufferReader reader);
    }
}
