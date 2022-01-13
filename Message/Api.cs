using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
	[ProtoContract]
	public partial class C2S_Login: IMessage
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
	public partial class R2C_Login: IMessage
	{
		[ProtoMember(1)]
		public string Address { get; set; }

		[ProtoMember(2)]
		public long Key { get; set; }

		[ProtoMember(3)]
		public ulong GateId { get; set; }

		[ProtoMember(4)]
		public List<ServerInfo> ServerList = new List<ServerInfo>();

	}

}
