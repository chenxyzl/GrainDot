using Akka.Actor;
using Base;
using Base.Helper;
using Common;
using Home.Model.Component;
using MongoDB.Bson;

namespace Home.Model;

public class Home : GameServer
{
    public Home() : base(RoleType.Home)
    {
    }

    public override void RegisterGlobalComponent()
    {
        AddComponent<TcpComponent>();
        AddComponent<WsComponent>();
        AddComponent<ConnectionDicCommponent>();
        var a = new IdGenerater(1).GenerateId();
        var b = new ObjectId(1.ToString());
        var c = b.ToString();
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