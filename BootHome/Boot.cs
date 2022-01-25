using System.Threading.Tasks;
using Base;
using Common;
using Home.Model;

namespace BootHome;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await GameServer.Run(typeof(Home.Model.Home), GameSharedRole.Player.ToString(), PlayerActor.P,
            MessageExtractor.PlayerMessageExtractor);
    }
}