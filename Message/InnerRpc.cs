using System.Collections.Generic;

namespace Message;

public partial class RpcItemMessage
{
    public static List<RpcItem> rpcItemsInner = new()
    {
        new RpcItem(10000, OpType.CS, typeof(AHPlayerLoginKeyAsk), typeof(HAPlayerLoginKeyAns),
            "PlayerLoginKeyHandler"),
        new RpcItem(10001, OpType.CS, typeof(HWPlayerOnlineAsk), typeof(WHPlayerOnlineAns),
            "PlayerOnlineHandler"),
        new RpcItem(10002, OpType.CS, typeof(HWPlayerOfflineAsk), typeof(WHPlayerOfflineAns),
            "PlayerOfflineHandler")
    };
}