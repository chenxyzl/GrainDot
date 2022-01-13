using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
// tcp
	[ProtoContract]
	public partial class OutRequest: IMessage
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
	public partial class OutResponse: IMessage
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

	[ProtoContract]
	public partial class C2G_Ping: IRequest
	{
	}

	[ProtoContract]
	public partial class G2C_Pong: IResponse
	{
	}

	[ProtoContract]
	public partial class C2M_Reload: IRequest
	{
		[ProtoMember(1)]
		public string Account { get; set; }

		[ProtoMember(2)]
		public string Password { get; set; }

	}

	[ProtoContract]
	public partial class M2C_Reload: IResponse
	{
	}

}
