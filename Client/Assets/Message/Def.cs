using System.Collections.Generic;
using ProtoBuf;

namespace Message
{
    [ProtoContract]
    public enum ItemType
    {
        Currency = 0,

        Item = 1,

        Hero = 2,

        Equip = 3,
    }

    [ProtoContract]
    public enum CurrencyType
    {
        Placeholder = 0,

        Gold = 1,

        Diamond = 2,

        Exp = 3,
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