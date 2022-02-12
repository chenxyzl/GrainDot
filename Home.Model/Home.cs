using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Common;

namespace Home.Model;

public class Home : GameServer
{
    public Home() : base(RoleType.Home)
    {
    }

    protected override async Task AfterCreate()
    {
        await base.AfterCreate();
        StartPlayerShardProxy();
    }
}

public static class GameServerExt
{
    public static Home GetHome(this GameServer _)
    {
        return GameServer.Instance as Home;
    }
}