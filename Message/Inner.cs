using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
	[ProtoContract]
	public partial class HWPlayerOnlineAsk: IActorRequest
	{
		[ProtoMember(1)]
		public ulong Uid { get; set; }

		[ProtoMember(2)]
		public long LoginTime { get; set; }

	}

	[ProtoContract]
	public partial class WHPlayerOnlineAns: IActorResponse
	{
	}

	[ProtoContract]
	public partial class HWPlayerOfflineAsk: IActorRequest
	{
		[ProtoMember(1)]
		public ulong Uid { get; set; }

		[ProtoMember(2)]
		public long OfflineTime { get; set; }

	}

	[ProtoContract]
	public partial class WHPlayerOfflineAns: IActorResponse
	{
	}

}
