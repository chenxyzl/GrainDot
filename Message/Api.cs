using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
	[ProtoContract]
	public partial class C2ALogin: IMessage
	{
		[ProtoMember(1)]
		public int MobileType { get; set; }

		[ProtoMember(2)]
		public string DeviceId { get; set; }

		[ProtoMember(3)]
		public string Account { get; set; }

		[ProtoMember(4)]
		public string Password { get; set; }

	}

	[ProtoContract]
	public partial class A2CLogin: IMessage
	{
		[ProtoMember(1)]
		public string Address { get; set; }

		[ProtoMember(2)]
		public long Key { get; set; }

	}

}
