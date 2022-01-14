using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
// tcp
	[ProtoContract]
	public partial class Request: IMessage
	{
		[ProtoMember(1)]
		public uint Opcode { get; set; }

		[ProtoMember(2)]
		public uint Sn { get; set; }

		[ProtoMember(3)]
		public byte[] Content { get; set; }

		[ProtoMember(4)]
		public string Sign { get; set; }

	}

// tcp
	[ProtoContract]
	public partial class Response: IMessage
	{
		[ProtoMember(1)]
		public uint Opcode { get; set; }

		[ProtoMember(2)]
		public uint Sn { get; set; }

		[ProtoMember(3)]
		public byte[] Content { get; set; }

		[ProtoMember(4)]
		public string Sign { get; set; }

		[ProtoMember(5)]
		public Code Code { get; set; }

	}

// http
	[ProtoContract]
	public partial class ApiResult: IMessage
	{
		[ProtoMember(1)]
		public Code Code { get; set; }

		[ProtoMember(2)]
		public string Msg { get; set; }

		[ProtoMember(3)]
		public byte[] Content { get; set; }

	}

}
