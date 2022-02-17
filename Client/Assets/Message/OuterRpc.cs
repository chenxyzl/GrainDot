using System.Collections.Generic;

namespace Message
{
    public partial class RpcItemMessage
    {
        public static List<RpcItem> rpcItemsOuter = new()
        {
            new RpcItem(200000, OpType.CS, typeof(C2SPing), typeof(S2CPong), "Ping"),
            new RpcItem(200001, OpType.C, typeof(CNotifyTest), null, "NotifyTest"),
            new RpcItem(200002, OpType.S, null, typeof(SPushTest), "PushTest"),
            new RpcItem(200003, OpType.CS, typeof(C2SLogin), typeof(S2CLogin), "Login"),
            new RpcItem(200004, OpType.S, null, typeof(SLoginElsewhere), "LoginElsewhere"),
            new RpcItem(200100, OpType.S, null, typeof(SSyncReward), "SyncReward"),
            new RpcItem(200101, OpType.CS, typeof(C2SMails), typeof(S2SMails), "GetMails")
        };
    }
}