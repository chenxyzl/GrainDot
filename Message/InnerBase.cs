using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
	[ProtoContract]
	public partial class RpcMessage: IMessage
	{
		[ProtoMember(1)]
		public uint Opcode { get; set; }

		[ProtoMember(2)]
		public byte[] Content { get; set; }

		[ProtoMember(3)]
		public ulong ActorId { get; set; }

		[ProtoMember(4)]
		public Code Code { get; set; }

	}

}
