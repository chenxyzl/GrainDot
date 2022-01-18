using Base;
using Base.Helper;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BootHome
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Game.Boot("PlayerActor", RoleDef.Home, typeof(Home.Model.Home));
        }
    }
}
