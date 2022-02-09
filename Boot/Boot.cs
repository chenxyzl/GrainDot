using System;
using System.Threading.Tasks;
using Base;
using Base.Helper;
using Common;
using Home.Model;
using World.Model;

namespace Boot;

internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length == 0) throw new Exception("参数里需要有启动类型");

        var roleType = EnumHelper.FromString<RoleType>(args[0]);
        switch (roleType)
        {
            case RoleType.Home:
            {
                await GameServer.Run(typeof(Home.Model.Home), GameSharedType.Player, PlayerActor.P,
                    MessageExtractor.PlayerMessageExtractor);
                break;
            }
            case RoleType.World:
            {
                await GameServer.Run(typeof(World.Model.World), GameSharedType.World, WorldActor.P,
                    MessageExtractor.WorldMessageExtractor);
                break;
            }
            case RoleType.Login:
            {
                await GameServer.Run(typeof(Login.Model.Login));
                break;
            }
            default:
            {
                throw new Exception($"{roleType} not found");
            }
        }
    }
}