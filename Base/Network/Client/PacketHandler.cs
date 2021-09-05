using Base.Network.Client.Interfaces;
using Base.Network.Shared.Interfaces;

namespace Base.Network.Client
{
    /// <inheritdoc/>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <inheritdoc/>
        public abstract void HandleMessageData(IBufferReader reader);
    }
}
