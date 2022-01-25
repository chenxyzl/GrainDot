using System.Threading.Tasks;
using Base;
using Common;
using World.Model;

namespace BootWorld;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await GameServer.Run(typeof(World.Model.World), GameSharedRole.World.ToString(), WorldActor.P,
            MessageExtractor.WorldMessageExtractor);
    }
}