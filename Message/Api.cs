using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
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

//登录选角色界面
	[ProtoContract]
	public partial class C2ALogin: IRequest
	{
		[ProtoMember(1)]
		public int MobileType { get; set; }

		[ProtoMember(2)]
		public string DeviceId { get; set; }

		[ProtoMember(3)]
		public string Token { get; set; }

	}

	[ProtoContract]
	public partial class A2CLogin: IResponse
	{
		[ProtoMember(1)]
		public List<SimpleRole> Rols = new List<SimpleRole>();

	}

//角色登录验证
	[ProtoContract]
	public partial class C2ARoleLogin: IRequest
	{
		[ProtoMember(1)]
		public ulong Uid { get; set; }

		[ProtoMember(2)]
		public string Token { get; set; }

	}

	[ProtoContract]
	public partial class A2CRoleLogin: IResponse
	{
		[ProtoMember(1)]
		public string Addr { get; set; }

		[ProtoMember(2)]
		public string Key { get; set; }

	}

	[ProtoContract]
	public partial class C2APerformanceTest: IRequest
	{
	}

	[ProtoContract]
	public partial class A2CPerformanceTest: IResponse
	{
	}

}
