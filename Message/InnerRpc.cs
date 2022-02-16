using System.Collections.Generic;

namespace Message;
public partial class RpcItemMessage
{
	public static List<RpcItem> rpcItemsInner = new()
	{
		new RpcItem(10000, OpType.CS, typeof(Message.AHPlayerLoginKeyAsk), typeof(Message.HAPlayerLoginKeyAns), "PlayerLoginKeyHandler"),
		new RpcItem(10001, OpType.CS, typeof(Message.HWPlayerOnlineAsk), typeof(Message.WHPlayerOnlineAns), "PlayerOnlineHandler"),
		new RpcItem(10002, OpType.CS, typeof(Message.HWPlayerOfflineAsk), typeof(Message.WHPlayerOfflineAns), "PlayerOfflineHandler"),
	};
}
