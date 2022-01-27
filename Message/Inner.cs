using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
	[ProtoContract]
	public partial class AHPlayerLoginKeyAsk: IRequestPlayer
	{
		[ProtoMember(1)]
		public ulong PlayerId { get; set; }

	}

	[ProtoContract]
	public partial class HAPlayerLoginKeyAns: IResponse
	{
		[ProtoMember(1)]
		public string PlayerKey { get; set; }

	}

	[ProtoContract]
	public partial class HWPlayerOnlineAsk: IRequestWorld
	{
		[ProtoMember(1)]
		public ulong WorldId { get; set; }

		[ProtoMember(2)]
		public ulong Uid { get; set; }

		[ProtoMember(3)]
		public long LoginTime { get; set; }

	}

	[ProtoContract]
	public partial class WHPlayerOnlineAns: IResponse
	{
	}

	[ProtoContract]
	public partial class HWPlayerOfflineAsk: IRequestWorld
	{
		[ProtoMember(1)]
		public ulong WorldId { get; set; }

		[ProtoMember(2)]
		public ulong Uid { get; set; }

		[ProtoMember(3)]
		public long OfflineTime { get; set; }

	}

	[ProtoContract]
	public partial class WHPlayerOfflineAns: IResponse
	{
	}

}
