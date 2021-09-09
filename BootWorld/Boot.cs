using Base;
using Base.Helper;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BootWorld
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Game.Boot(RoleDef.World, typeof(World.Model.World));
        }
    }
}
