using Base;
using Base.Helper;
using Common;
using Home.Model;
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
            await GameServer.Run(typeof(Home.Model.Home), GameSharedRole.Player.ToString(), PlayerActor.P, MessageExtractor.PlayerMessageExtractor);
        }
    }
}
