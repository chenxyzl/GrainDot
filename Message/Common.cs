using ProtoBuf;
using System.Collections.Generic;
namespace cc
{
	[ProtoContract]
	public enum ItemType
	{
		Currency = 0,

		Equip = 1,

		Artifact = 2,

		Hero = 3,

		SyntheticMaterials = 10,

		ExpMaterials = 11,

		HeroFragments = 12,

		Gift = 20,

		WorldInnerMaterials = 90,

	}

	[ProtoContract]
	public enum CurrencyType
	{
		Placeholder = 0,

		Exp = 1,

		Gold = 2,

		Diamond = 3,

		HeroExp = 4,

		EquipExp = 5,

		ArtifactExp = 6,

		CareerExp = 7,

	}

//服务器列表
	[ProtoContract]
	public partial class ServerInfo: IMessage
	{
		[ProtoMember(1)]
		public int TimeZone { get; set; }

		[ProtoMember(2)]
		public long TimeStamp { get; set; }

		[ProtoMember(3)]
		public int ServerID { get; set; }

		[ProtoMember(4)]
		public string ServerName { get; set; }

		[ProtoMember(5)]
		public long OpenServerTime { get; set; }

	}

	[ProtoContract]
	public partial class Hero: IMessage
	{
		[ProtoMember(1)]
		public int Id { get; set; }

		[ProtoMember(2)]
		public int Lv { get; set; }

		[ProtoMember(3)]
		public int Exp { get; set; }

		[ProtoMember(4)]
		public int QualityLv { get; set; }

		[ProtoMember(5)]
		public int StarLv { get; set; }

		[ProtoMember(6)]
		public int BtLv { get; set; }

		[ProtoMember(7)]
		public ulong EquipId { get; set; }

	}

	[ProtoContract]
	public partial class Item: IMessage
	{
		[ProtoMember(1)]
		public int Id { get; set; }

		[ProtoMember(2)]
		public long Count { get; set; }

	}

	[ProtoContract]
	public partial class BtCard: IMessage
	{
		[ProtoMember(1)]
		public ulong Id { get; set; }

		[ProtoMember(2)]
		public int Tid { get; set; }

		[ProtoMember(3)]
		public long Lv { get; set; }

		[ProtoMember(4)]
		public int Exp { get; set; }

	}

	[ProtoContract]
	public partial class Equip: IMessage
	{
		[ProtoMember(1)]
		public ulong Id { get; set; }

		[ProtoMember(2)]
		public int Tid { get; set; }

		[ProtoMember(3)]
		public int Lv { get; set; }

		[ProtoMember(4)]
		public int Exp { get; set; }

		[ProtoMember(5)]
		public int StarLv { get; set; }

	}

	[ProtoContract]
	public enum CareerType
	{
// 战士
		Warror = 0,

//弓箭手
		Arrow = 1,

//法师
		Master = 2,

//剑士
		Swordsman = 3,

//剑圣
		SwordHoly = 4,

	}

//稀有度
	[ProtoContract]
	public enum Rarity
	{
		White = 0,

		Green = 1,

		Blue = 2,

		Purple = 3,

		Orange = 4,

		Red = 5,

	}

}
