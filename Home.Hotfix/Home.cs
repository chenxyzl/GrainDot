using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Hotfix
{
    public class Home : GameServer
    {
        #region 单例
        private Home() : base(Common.RoleDef.Home) { }

        private static readonly Home singleInstance = new Home();

        public static Home Instance { get { return singleInstance; } }
        #endregion
    }
}
