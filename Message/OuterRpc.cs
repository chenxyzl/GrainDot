using System.Collections.Generic;

namespace Message;

public partial class RpcItemMessage
{
    public static List<RpcItem> rpcItemsOuter = new()
    {
        new(200000, OpType.CS, typeof(C2SPing), typeof(S2CPong), "Ping"),
        new(200001, OpType.C, typeof(CNotifyTest), null, "NotifyTest"),
        new(200002, OpType.S, null, typeof(SPushTest), "PushTest"),
        new(200003, OpType.CS, typeof(C2SLogin), typeof(S2CLogin), "Login"),
        new(200004, OpType.S, null, typeof(SSyncReward), "SyncReward"),
        new(200005, OpType.CS, typeof(C2SMails), typeof(S2SMails), "GetMails")
    };
}