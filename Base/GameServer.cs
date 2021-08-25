using Base.Alg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class GameServer
    {
        public readonly ILog logger;
        public GameServer(Common.RoleDef role)
        {
            logger = new NLogAdapter(role.ToString());
        }
        public void test()
        {
            logger.Warning("xxxxx");
        }
    }
}
