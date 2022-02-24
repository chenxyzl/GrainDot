using System.Net;
using System.Threading.Tasks;
using Base;
using Common;
using Home.Model.Component;
using Share.Model.Component;

namespace Home.Model;

public class Home : GameServer
{
    public Home(ushort nodeId) : base(RoleType.Home, nodeId)
    {
    }

    public new static Home Instance => A.NotNull(_ins as Home);

    protected override async Task AfterCreate()
    {
        await base.AfterCreate();
        StartPlayerShardProxy();
    }

    protected override void RegisterComponent()
    {
        GameServer.Instance.AddComponent<TcpComponent>(IPAddress.Parse("0.0.0.0"), (ushort) 15000);
        GameServer.Instance.AddComponent<WsComponent>(IPAddress.Parse("0.0.0.0"), (ushort) 15001);
        GameServer.Instance.AddComponent<ConnectionComponent>();
        GameServer.Instance.AddComponent<ConsoleComponent>();
        GameServer.Instance.AddComponent<ReplComponent>();
        GameServer.Instance.AddComponent<LoginKeyComponent>();
        GameServer.Instance.AddComponent<DBComponent>("mongodb://root:Qwert123!@10.7.69.254:27017");
        GameServer.Instance.AddComponent<EtcdComponent>(
            "http://10.7.69.254:12379,http://10.7.69.254:22379,http://10.7.69.254:32379");
    }
}