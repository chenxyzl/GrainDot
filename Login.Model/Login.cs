using System.Threading.Tasks;
using Base;
using Base.Network.Http;
using Common;

namespace Login.Model;

public class Login : GameServer
{
    public Login() : base(RoleType.Login)
    {
    }

    public override void RegisterGlobalComponent()
    {
        AddComponent<HttpComponent>(":20001");
    }
    
    protected override async Task AfterCreate()
    {
        await base.AfterCreate();
        StartPlayerShardProxy();
    }
}