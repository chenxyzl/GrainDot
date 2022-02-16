using System.Collections.Generic;
using ProtoBuf;
namespace Message;
// api服务器获取玩家的登录的key
[ProtoContract]
public partial class AHPlayerLoginKeyAsk: IRequest
{
}

[ProtoContract]
public partial class HAPlayerLoginKeyAns: IResponse
{
	[ProtoMember(1)]
	public string PlayerKey { get; set; }

}

// 玩家上线
[ProtoContract]
public partial class HWPlayerOnlineAsk: IRequest
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

// 玩家下线
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

