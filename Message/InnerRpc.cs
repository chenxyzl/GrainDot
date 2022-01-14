namespace 
{
	public partial class OpcodeTypeComponent
	{
		public void Load()
		{
			AddRpcInfo(10000, OpType.SS, typeof(Message.HWPlayerOnlineAsk), typeof(Message.WHPlayerOnlineAns));
			AddRpcInfo(10001, OpType.SS, typeof(Message.HWPlayerOfflineAsk), typeof(Message.WHPlayerOfflineAns));
		}
	}
}
