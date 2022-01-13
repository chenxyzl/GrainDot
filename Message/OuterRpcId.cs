using ProtoBuf;
using System.Collections.Generic;
namespace cc
{
//消息范围10000~19999
	[ProtoContract]
	public enum OuterRpcId
	{
		Ping = 10000,

		CPushTest = 10001,

		SPushTest = 10002,

		Login = 10003,

	}

}
