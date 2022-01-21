using Base;
using Base.Helper;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using World.Model;

namespace BootWorld
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await GameServer.Run(typeof(World.Model.World), GameSharedRole.World.ToString(), WorldActor.P, MessageExtractor.WorldMessageExtractor);
        }
    }
}
