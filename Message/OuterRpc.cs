namespace 
{
	public partial class OpcodeTypeComponent
	{
		public void Load()
		{
			AddRpcInfo(200000, OpType.CS, typeof(cc.C2SPing), typeof(cc.S2CPong));
			AddRpcInfo(200001, OpType.C, typeof(cc.CNotifyTest), null);
			AddRpcInfo(200002, OpType.S, null, typeof(cc.SPushTest));
			AddRpcInfo(200003, OpType.CS, typeof(cc.C2SLogin), typeof(cc.S2CLogin));
			AddRpcInfo(200004, OpType.S, null, typeof(cc.SSyncReward));
			AddRpcInfo(200005, OpType.CS, typeof(cc.C2SMails), typeof(cc.S2SMails));
		}
	}
}
