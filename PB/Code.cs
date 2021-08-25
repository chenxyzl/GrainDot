using ProtoBuf;
using System.Collections.Generic;
namespace PB
{
	[ProtoContract]
	public enum Code
	{
		Ok = 0,

		Error = 1,

		GateSignFailed = 2,

		AccountIllegal = 3,

		GateNotFound = 4,

		CareerHasUnlock = 5,

		CareerNotUnlock = 6,

		CareerTypeNotExist = 7,

		CareerConditionsLimit = 8,

		ItemIdNotExist = 9,

		ItemTypeNotDeal = 10,

		ItemNotCurrency = 11,

		PlayerOffline = 12,

		AddCountMustBiggerThan0 = 13,

		MustSetWorldId = 14,

		ConfigNotFound = 15,

	}

}
