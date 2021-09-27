namespace Message
{
	public partial class OpcodeTypeComponent
	{
		public void LoadInnerOpcode()
		{
			AddRpcInfo(1000, OpType.RR, typeof(Message.H2L_PlayerOnline), typeof(Message.L2H_PlayerOnline));
			AddRpcInfo(1002, OpType.RR, typeof(Message.H2L_PlayerOffline), typeof(Message.L2H_PlayerOffline));
			AddRpcInfo(1004, OpType.RR, typeof(Message.L2H_PlayerKickOut), typeof(Message.H2L_PlayerKickOut));
			AddRpcInfo(1006, OpType.RR, typeof(Message.L2A_PlayerOnline), null);
			AddRpcInfo(1008, OpType.RR, typeof(Message.L2A_PlayerOffline), null);
			AddRpcInfo(11000, OpType.RR, typeof(Message.R2G_GetLoginKey), typeof(Message.G2R_GetLoginKey));
		}
	}
}
