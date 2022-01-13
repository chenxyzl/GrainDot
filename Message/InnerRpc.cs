namespace 
{
	public partial class OpcodeTypeComponent
	{
		public void Load()
		{
			AddRpcInfo(10000, OpType.SS, typeof(cc.HWPlayerOnlineAsk), typeof(cc.WHPlayerOnlineAns));
			AddRpcInfo(10001, OpType.SS, typeof(cc.HWPlayerOfflineAsk), typeof(cc.WHPlayerOfflineAns));
			AddRpcInfo(10002, OpType.SS, typeof(cc.HWPlayerKickOutAsk), typeof(cc.WHPlayerKickOutAns));
		}
	}
}
