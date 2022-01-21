using Message;
namespace Base
{
    public partial class RpcManager
    {
        void AddInnerRpcItem()
        {
            AddInnerRpcItem(new RpcItem(10000, RpcType.CS, typeof(AHPlayerLoginKeyAsk), typeof(HAPlayerLoginKeyAns), "PlayerLoginKeyHandler"));
            AddInnerRpcItem(new RpcItem(10001, RpcType.CS, typeof(HWPlayerOnlineAsk), typeof(HAPlayerLoginKeyAns), "PlayerOnlineHandler"));
            AddInnerRpcItem(new RpcItem(10002, RpcType.CS, typeof(HWPlayerOfflineAsk), typeof(HAPlayerLoginKeyAns), "PlayerOfflineHandler"));
        }
    }
}
