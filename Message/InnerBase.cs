using ProtoBuf;
using System.Collections.Generic;
namespace cc
{
	[ProtoContract]
	public partial class RpcMessage: IMessage
	{
		[ProtoMember(1)]
		public uint Opcode { get; set; }

		[ProtoMember(2)]
		public byte[] Content { get; set; }

		[ProtoMember(3)]
		public Code Code { get; set; }

	}

}
