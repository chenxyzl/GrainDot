using System.Threading.Tasks;
using Base;

namespace BootLogin;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await GameServer.Run(typeof(Login.Model.Login));
    }
}