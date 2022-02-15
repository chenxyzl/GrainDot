using System.Threading.Tasks;
using Base;
using Common;

namespace Home.Model;

public class Home : GameServer
{
    public Home() : base(RoleType.Home)
    {
    }

    public new static Home Instance => A.NotNull(_ins as Home);

    protected override async Task AfterCreate()
    {
        await base.AfterCreate();
        StartPlayerShardProxy();
    }
}