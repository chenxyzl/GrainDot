using System.Collections.Generic;
using ProtoBuf;
namespace Message;
// 请求玩家的消息包装
[ProtoContract]
public partial class RequestPlayer: IRequestPlayer
{
	[ProtoMember(1)]
	public uint Opcode { get; set; }

	[ProtoMember(2)]
	public ulong Sn { get; set; }

	[ProtoMember(3)]
	public ulong PlayerId { get; set; }

	[ProtoMember(4)]
	public byte[] Content { get; set; }

}

// 请求World的消息包装
[ProtoContract]
public partial class RequestWorld: IRequestWorld
{
	[ProtoMember(1)]
	public uint Opcode { get; set; }

	[ProtoMember(2)]
	public ulong Sn { get; set; }

	[ProtoMember(3)]
	public ulong WorldId { get; set; }

	[ProtoMember(4)]
	public byte[] Content { get; set; }

}

//消息返回
[ProtoContract]
public partial class InnerResponse: IResponse
{
	[ProtoMember(1)]
	public uint Opcode { get; set; }

	[ProtoMember(2)]
	public ulong Sn { get; set; }

	[ProtoMember(3)]
	public byte[] Content { get; set; }

	[ProtoMember(4)]
	public Code Code { get; set; }

}

//actor线程恢复
[ProtoContract]
public partial class ResumeActor: IRequestPlayer
{
	[ProtoMember(1)]
	public ulong Sn { get; set; }

	[ProtoMember(2)]
	public ulong PlayerId { get; set; }

}

