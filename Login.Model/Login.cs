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

    protected override async Task AfterCreate()
    {
        await base.AfterCreate();
        StartPlayerShardProxy();
    }
}