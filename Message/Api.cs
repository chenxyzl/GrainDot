using System.Collections.Generic;
using ProtoBuf;
namespace Message;
//角色简单信息
[ProtoContract]
public partial class SimpleRole: IMessage
{
	[ProtoMember(1)]
	public ulong Uid { get; set; }

	[ProtoMember(2)]
	public uint Tid { get; set; }

	[ProtoMember(3)]
	public string Name { get; set; }

	[ProtoMember(4)]
	public long LastLoginTime { get; set; }

	[ProtoMember(5)]
	public long LastOfflineTime { get; set; }

	[ProtoMember(6)]
	public ulong Exp { get; set; }

}

//获取角色列表
[ProtoContract]
public partial class C2AGetRoleList: IHttpRequest
{
	[ProtoMember(1)]
	public int MobileType { get; set; }

	[ProtoMember(2)]
	public string DeviceId { get; set; }

	[ProtoMember(3)]
	public string Token { get; set; }

}

[ProtoContract]
public partial class A2CGetRoleList: IHttpResponse
{
		[ProtoMember(1)]
		public List<SimpleRole> Rols = new List<SimpleRole>();

}

//角色登录验证
[ProtoContract]
public partial class C2ARoleLogin: IHttpRequest
{
	[ProtoMember(1)]
	public ulong Uid { get; set; }

	[ProtoMember(2)]
	public string Token { get; set; }

}

[ProtoContract]
public partial class A2CRoleLogin: IHttpResponse
{
	[ProtoMember(1)]
	public string Addr { get; set; }

	[ProtoMember(2)]
	public string Key { get; set; }

}

//性能测试
[ProtoContract]
public partial class C2APerformanceTest: IHttpRequest
{
}

[ProtoContract]
public partial class A2CPerformanceTest: IHttpResponse
{
}

