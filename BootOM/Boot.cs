using System.Threading.Tasks;
using Base;

namespace BootOM;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await GameServer.Run(typeof(OM.Model.OM));
    }
}