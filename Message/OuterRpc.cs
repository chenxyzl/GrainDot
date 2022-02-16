using System.Collections.Generic;

namespace Message;
public partial class RpcItemMessage
{
	public static List<RpcItem> rpcItemsOuter = new()
	{
		new RpcItem(200000, OpType.CS, typeof(Message.C2SPing), typeof(Message.S2CPong), "Ping"),
		new RpcItem(200001, OpType.C, typeof(Message.CNotifyTest), null, "NotifyTest"),
		new RpcItem(200002, OpType.S, null, typeof(Message.SPushTest), "PushTest"),
		new RpcItem(200003, OpType.CS, typeof(Message.C2SLogin), typeof(Message.S2CLogin), "Login"),
		new RpcItem(200004, OpType.S, null, typeof(Message.SLoginElsewhere), "LoginElsewhere"),
		new RpcItem(200100, OpType.S, null, typeof(Message.SSyncReward), "SyncReward"),
		new RpcItem(200101, OpType.CS, typeof(Message.C2SMails), typeof(Message.S2SMails), "GetMails"),
	};
}
