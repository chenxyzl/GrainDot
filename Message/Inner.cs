using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
	[ProtoContract]
	public partial class HWPlayerOnlineAsk: IRequest
	{
		[ProtoMember(1)]
		public ulong Uid { get; set; }

		[ProtoMember(2)]
		public long LoginTime { get; set; }

	}

	[ProtoContract]
	public partial class WHPlayerOnlineAns: IResponse
	{
	}

	[ProtoContract]
	public partial class HWPlayerOfflineAsk: IRequest
	{
		[ProtoMember(1)]
		public ulong Uid { get; set; }

		[ProtoMember(2)]
		public long OfflineTime { get; set; }

	}

	[ProtoContract]
	public partial class WHPlayerOfflineAns: IResponse
	{
	}

}
