using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
	[ProtoContract]
	public partial class C2SPing: IRequest
	{
	}

	[ProtoContract]
	public partial class S2CPong: IResponse
	{
		[ProtoMember(1)]
		public long Time { get; set; }

	}

	[ProtoContract]
	public partial class CNotifyTest: IRequest
	{
	}

	[ProtoContract]
	public partial class SPushTest: IMessage
	{
	}

//游戏服务器的登录 第一条消息 从这里开始
	[ProtoContract]
	public partial class C2SLogin: IRequest
	{
		[ProtoMember(1)]
		public string Key { get; set; }

		[ProtoMember(2)]
		public string Unused { get; set; }

	}

	[ProtoContract]
	public partial class S2CLogin: IResponse
	{
		[ProtoMember(1)]
		public Role Role { get; set; }

	}

	[ProtoContract]
	public partial class Role: IMessage
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

		[ProtoMember(4)]
		public ulong Exp { get; set; }

		[ProtoMember(6)]
		public Dictionary<int,ulong> CurrencyBag = new Dictionary<int,ulong>();

		[ProtoMember(7)]
		public List<Item> ItemBag = new List<Item>();

		[ProtoMember(8)]
		public List<Hero> HeroBag = new List<Hero>();

		[ProtoMember(9)]
		public List<Equip> EquipBag = new List<Equip>();

	}

	[ProtoContract]
	public partial class SSyncReward: IMessage
	{
		[ProtoMember(1)]
		public List<Item> Adds = new List<Item>();

		[ProtoMember(2)]
		public List<Item> Dels = new List<Item>();

	}

	[ProtoContract]
	public partial class Mail: IMessage
	{
		[ProtoMember(1)]
		public ulong Uid { get; set; }

		[ProtoMember(2)]
		public uint Tid { get; set; }

		[ProtoMember(3)]
		public string CustomTitle { get; set; }

		[ProtoMember(4)]
		public string CustomContent { get; set; }

		[ProtoMember(5)]
		public List<string> Params = new List<string>();

		[ProtoMember(6)]
		public long RecvTime { get; set; }

		[ProtoMember(7)]
		public bool HasRead { get; set; }

		[ProtoMember(8)]
		public bool HasGet { get; set; }

	}

	[ProtoContract]
	public partial class C2SMails: IRequest
	{
	}

	[ProtoContract]
	public partial class S2SMails: IResponse
	{
		[ProtoMember(1)]
		public List<Mail> Mails = new List<Mail>();

	}

}
