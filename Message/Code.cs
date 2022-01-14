using ProtoBuf;
using System.Collections.Generic;
namespace Message
{
	[ProtoContract]
	public enum Code
	{
		Ok = 0,

		Error = 1,

		GateSignFailed = 2,

		AccountIllegal = 3,

		ItemNodEnough = 4,

		ItemIdNotExist = 5,

		ItemTypeNotDeal = 6,

		AddCountMustBiggerThan0 = 7,

		ConfigNotFound = 8,

	}

}
