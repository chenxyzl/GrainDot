using Base.Network.Server.Interfaces;
using Base.Network.Shared.Interfaces;

namespace Base.Network.Server
{
    /// <inheritdoc/>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <inheritdoc/>
        public abstract void HandleMessageData(IClient client, IBufferReader reader);
    }
}
