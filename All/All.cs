using Base;
using System;

namespace All
{
    public sealed class All : GameServer
    {
        #region 单例
        private All() : base(Common.RoleDef.All) { }

        private static readonly All singleInstance = new All();

        public static All Instance { get { return singleInstance; } }
        #endregion
    }
}
