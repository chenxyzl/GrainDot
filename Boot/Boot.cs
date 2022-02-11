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
        if (CommandHelper.Instance.Parse(args))
            return;

        var roleType = EnumHelper.FromString<RoleType>(CommandHelper.Instance.NodeType);
        var nodeId = uint.Parse(CommandHelper.Instance.NodeId);
        switch (roleType)
        {
            case RoleType.Home:
            {
                await GameServer.Run(typeof(Home.Model.Home), GameSharedType.Player, PlayerActor.P,
                    MessageExtractor.PlayerMessageExtractor, nodeId);
                break;
            }
            case RoleType.World:
            {
                await GameServer.Run(typeof(World.Model.World), GameSharedType.World, WorldActor.P,
                    MessageExtractor.WorldMessageExtractor, nodeId);
                break;
            }
            case RoleType.Login:
            {
                await GameServer.Run(typeof(Login.Model.Login), nodeId);
                break;
            }
            default:
            {
                throw new Exception($"{roleType} not found");
            }
        }
    }
}