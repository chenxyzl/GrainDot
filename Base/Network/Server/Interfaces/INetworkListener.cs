using Base.Network.Shared;
using Base.Network.Shared.Interfaces;

namespace Base.Network.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client.
    /// </summary>
    public interface INetworkListener
    {
        #region public methods implementation

        /// <summary>
        /// This method is responsible for call the dispose implementation method.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Method responsible for stop the tcp network listener.
        /// </summary>
        void Stop();

        #endregion
    }
}