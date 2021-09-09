using Base;
using System;

namespace All
{
    public sealed class All : GameServer
    {
        private All() : base(Common.RoleDef.All) { }

        private static readonly All singleInstance = new All();

        public static All GetInstance { get { return singleInstance; } }
    }
}
