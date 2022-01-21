using Message;
namespace Base
{
    public partial class RpcManager
    {
        void AddOuterRpcItem()
        {
            AddInnerRpcItem(new RpcItem(200000, RpcType.CS, typeof(C2SPing), typeof(S2CPong), "Ping"));
            AddInnerRpcItem(new RpcItem(200001, RpcType.C, typeof(CNotifyTest), null, "NotifyTest"));
            AddInnerRpcItem(new RpcItem(200002, RpcType.S, null, typeof(SPushTest), "PushTest"));
            AddInnerRpcItem(new RpcItem(200003, RpcType.CS, typeof(C2SLogin), typeof(S2CLogin), "Login"));
            AddInnerRpcItem(new RpcItem(200004, RpcType.S, null, typeof(SSyncReward), "SyncReward"));
            AddInnerRpcItem(new RpcItem(200005, RpcType.CS, typeof(C2SMails), typeof(C2SMails), "GetMails"));
        }
    }
}
