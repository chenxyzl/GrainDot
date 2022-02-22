using System.Threading.Tasks;
using Base;
using Common;
using Share.Model.Component;

namespace Login.Model;

public class Login : GameServer
{
    public Login(ushort nodeId) : base(RoleType.Login, nodeId)
    {
    }


    protected override async Task AfterCreate()
    {
        await base.AfterCreate();
        StartPlayerShardProxy();
    }

    protected override void RegisterComponent()
    {
        GameServer.Instance.AddComponent<DBComponent>("mongodb://root:Qwert123!@10.7.69.254:27017");
        GameServer.Instance.AddComponent<HttpComponent>(":20001");
    }
}