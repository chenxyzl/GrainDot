using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
// 客户端请求服务器的消息体
	[ProtoContract]
	public partial class Request: IRequest
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

// 服务器返回客户端的消息体
	[ProtoContract]
	public partial class Response: IResponse
	{
		[ProtoMember(1)]
		public uint Opcode { get; set; }

		[ProtoMember(2)]
		public uint Sn { get; set; }

		[ProtoMember(3)]
		public byte[] Content { get; set; }

		[ProtoMember(4)]
		public Code Code { get; set; }

	}

// http请求的返回值
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
