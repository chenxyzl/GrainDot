using ProtoBuf;
using System.Collections.Generic;
namespace cc
{
	[ProtoContract]
	public partial class C_PushTest: IMessage
	{
		[ProtoMember(1)]
		public string V { get; set; }

	}

//测试
	[ProtoContract]
	public partial class S_PushTest: IMessage
	{
		[ProtoMember(1)]
		public string V { get; set; }

	}

//登录游戏服务器
	[ProtoContract]
	public partial class C2S_Login: IRequest
	{
		[ProtoMember(1)]
		public long Key { get; set; }

		[ProtoMember(2)]
		public ulong GateId { get; set; }

		[ProtoMember(3)]
		public bool IsReconnet { get; set; }

	}

	[ProtoContract]
	public partial class Formation: IMessage
	{
		[ProtoMember(1)]
		public List<int> Heros = new List<int>();

		[ProtoMember(2)]
		public List<ulong> Artifacts = new List<ulong>();

	}

	[ProtoContract]
	public partial class WorldData: IMessage
	{
		[ProtoMember(1)]
		public Dictionary<int,Item> InnerBag = new Dictionary<int,Item>();

	}

//玩家数据
	[ProtoContract]
	public partial class PlayerData: IMessage
	{
		[ProtoMember(1)]
		public ulong Id { get; set; }

		[ProtoMember(2)]
		public string Name { get; set; }

// int32 lv = 3; //等级用经验换算
		[ProtoMember(4)]
		public long Exp { get; set; }

		[ProtoMember(5)]
		public CareerType CurrentCareer { get; set; }

		[ProtoMember(6)]
		public Dictionary<int,long> CareerBag = new Dictionary<int,long>();

		[ProtoMember(7)]
		public Dictionary<int,Item> ItemBag = new Dictionary<int,Item>();

		[ProtoMember(8)]
		public Dictionary<int,Hero> HeroBag = new Dictionary<int,Hero>();

		[ProtoMember(9)]
		public Dictionary<ulong,Equip> EquipBag = new Dictionary<ulong,Equip>();

		[ProtoMember(10)]
		public Dictionary<ulong,BtCard> BtCardBag = new Dictionary<ulong,BtCard>();

		[ProtoMember(11)]
		public Dictionary<ulong,Equip> ArtifactBag = new Dictionary<ulong,Equip>();

		[ProtoMember(12)]
		public Formation Formation { get; set; }

	}

	[ProtoContract]
	public partial class S2C_Login: IResponse
	{
		[ProtoMember(1)]
		public PlayerData PlayerData { get; set; }

	}

	[ProtoContract]
	public partial class C2S_SelectCareer: IRequest
	{
		[ProtoMember(1)]
		public CareerType CareerType { get; set; }

	}

	[ProtoContract]
	public partial class S2C_SelectCareer: IResponse
	{
	}

//同步奖励数据变化
	[ProtoContract]
	public partial class S_GiveReward: IMessage
	{
		[ProtoMember(1)]
		public Dictionary<int,long> Items = new Dictionary<int,long>();

		[ProtoMember(2)]
		public List<Item> ItemsDisplay = new List<Item>();

	}

	[ProtoContract]
	public partial class C2S_CareerLvUp: IRequest
	{
		[ProtoMember(1)]
		public CareerType CareerType { get; set; }

	}

	[ProtoContract]
	public partial class S2C_CareerLvUp: IResponse
	{
		[ProtoMember(2)]
		public int Lv { get; set; }

	}

	[ProtoContract]
	public partial class C2S_HeroLvUp: IRequest
	{
		[ProtoMember(1)]
		public int HeroId { get; set; }

	}

	[ProtoContract]
	public partial class S2C_HeroLvUp: IResponse
	{
		[ProtoMember(1)]
		public int Lv { get; set; }

	}

	[ProtoContract]
	public partial class C2S_QualityUpgrade: IRequest
	{
		[ProtoMember(1)]
		public int HeroId { get; set; }

	}

	[ProtoContract]
	public partial class S2C_QualityUpgrade: IResponse
	{
		[ProtoMember(1)]
		public int Quality { get; set; }

	}

	[ProtoContract]
	public partial class C2S_StarUpgrade: IRequest
	{
		[ProtoMember(1)]
		public int HeroId { get; set; }

	}

	[ProtoContract]
	public partial class S2C_StarUpgrade: IResponse
	{
		[ProtoMember(1)]
		public int StarLv { get; set; }

	}

	[ProtoContract]
	public partial class C2S_BtUpgrade: IRequest
	{
		[ProtoMember(1)]
		public int HeroId { get; set; }

	}

	[ProtoContract]
	public partial class S2C_BtUpgrade: IResponse
	{
		[ProtoMember(1)]
		public int BtLv { get; set; }

	}

	[ProtoContract]
	public partial class C2S_EquipLvUp: IRequest
	{
		[ProtoMember(1)]
		public ulong EquipId { get; set; }

	}

	[ProtoContract]
	public partial class S2C_EquipLvUp: IResponse
	{
		[ProtoMember(1)]
		public int Lv { get; set; }

	}

	[ProtoContract]
	public partial class C2S_EquipStarUp: IRequest
	{
		[ProtoMember(1)]
		public ulong EquipId { get; set; }

	}

	[ProtoContract]
	public partial class S2C_EquipStarUp: IResponse
	{
		[ProtoMember(1)]
		public int StarLv { get; set; }

	}

	[ProtoContract]
	public partial class C2S_ArtifactLvUp: IRequest
	{
		[ProtoMember(1)]
		public ulong ArtifactId { get; set; }

	}

	[ProtoContract]
	public partial class S2C_ArtifactLvUp: IResponse
	{
		[ProtoMember(1)]
		public int Lv { get; set; }

	}

	[ProtoContract]
	public partial class C2S_ArtifactStarUp: IRequest
	{
		[ProtoMember(1)]
		public ulong ArtifactId { get; set; }

	}

	[ProtoContract]
	public partial class S2C_ArtifactStarUp: IResponse
	{
		[ProtoMember(1)]
		public int StarLv { get; set; }

	}

}
