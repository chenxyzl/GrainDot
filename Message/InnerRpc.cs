namespace Message
{
	public partial class OpcodeTypeComponent
	{
		public void Load()
		{
			AddRpcInfo(10000, OpType.SS, typeof(Message.AHPlayerLoginKeyAsk), typeof(Message.HAPlayerLoginKeyAns));
			AddRpcInfo(10001, OpType.SS, typeof(Message.HWPlayerOnlineAsk), typeof(Message.WHPlayerOnlineAns));
			AddRpcInfo(10002, OpType.SS, typeof(Message.HWPlayerOfflineAsk), typeof(Message.WHPlayerOfflineAns));
		}
	}
}
