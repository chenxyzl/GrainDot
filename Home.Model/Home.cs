using Akka.Actor;
using Base;
using Common;

namespace Home.Model;

public class Home : GameServer
{
    public Home() : base(RoleType.Home)
    {
    }

    public IActorRef GetLocalPlayerActorRef(ulong playerId)
    {
        //todo 拼路径
        var path = playerId.ToString();
        return GetChild(path);
    }
}

public static class GameServerExt
{
    public static Home GetHome(this GameServer _)
    {
        return GameServer.Instance as Home;
    }
}