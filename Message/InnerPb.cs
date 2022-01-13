using ProtoBuf;
using System.Collections.Generic;
namespace cc
{
	[ProtoContract]
	public partial class R2G_GetLoginKey: IActorRequest
	{
		[ProtoMember(1)]
		public string Account { get; set; }

		[ProtoMember(2)]
		public Dictionary<int,L2H_PlayerKickOut> A = new Dictionary<int,L2H_PlayerKickOut>();

	}

	[ProtoContract]
	public partial class G2R_GetLoginKey: IActorResponse
	{
		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public ulong GateId { get; set; }

	}

	[ProtoContract]
	public partial class H2L_PlayerOnline: IActorRequest
	{
		[ProtoMember(1)]
		public ulong PlayerId { get; set; }

		[ProtoMember(2)]
		public ulong HomeId { get; set; }

	}

	[ProtoContract]
	public partial class L2H_PlayerOnline: IActorResponse
	{
	}

	[ProtoContract]
	public partial class L2A_PlayerOnline: IActorMessage
	{
		[ProtoMember(1)]
		public ulong PlayerId { get; set; }

		[ProtoMember(2)]
		public ulong HomeId { get; set; }

	}

	[ProtoContract]
	public partial class H2L_PlayerOffline: IActorRequest
	{
		[ProtoMember(1)]
		public ulong PlayerId { get; set; }

		[ProtoMember(2)]
		public ulong HomeId { get; set; }

	}

	[ProtoContract]
	public partial class L2H_PlayerOffline: IActorResponse
	{
	}

	[ProtoContract]
	public partial class L2A_PlayerOffline: IActorMessage
	{
		[ProtoMember(1)]
		public ulong PlayerId { get; set; }

	}

	[ProtoContract]
	public partial class L2H_PlayerKickOut: IActorRequest
	{
		[ProtoMember(1)]
		public ulong PlayerId { get; set; }

	}

	[ProtoContract]
	public partial class H2L_PlayerKickOut: IActorResponse
	{
	}

}
