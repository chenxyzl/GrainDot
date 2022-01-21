namespace Message
{
	public partial class OpcodeTypeComponent
	{
		public void Load()
		{
			AddRpcInfo(10000, OpType.CS, typeof(Message.AHPlayerLoginKeyAsk), typeof(Message.HAPlayerLoginKeyAns));
			AddRpcInfo(10001, OpType.CS, typeof(Message.HWPlayerOnlineAsk), typeof(Message.WHomPlayerOnlineAns));
			AddRpcInfo(10002, OpType.CS, typeof(Message.HWPlayerOfflineAsk), typeof(Message.WHPlayerOfflineAns));
		}
	}
}
