using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
//消息范围10000~19999
//请求消息一定是偶数，返回消息id为请求消息+1
	[ProtoContract]
	public enum OuterRpcId
	{
		Login = 10000,

	}

}
