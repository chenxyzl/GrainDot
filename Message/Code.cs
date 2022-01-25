using ProtoBuf;

namespace Message;

[ProtoContract]
public enum Code
{
    Ok = 0,

    Error = 1,

    GateSignFailed = 2,

    AccountIllegal = 3,

    ItemNodEnough = 10000,

    ItemIdNotExist = 10001,

    ItemTypeNotDeal = 10002,

    AddCountMustBiggerThan0 = 10003,

    ConfigNotFound = 10004,

    ItemNotCurrency = 10005,

    PlayerNotOnline = 10006
}