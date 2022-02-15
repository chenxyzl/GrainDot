using System.Threading.Tasks;
using Base;
using Common;

namespace Home.Model;

public class Home : GameServer
{
    public new static Home Instance => A.NotNull(_ins as Home);

    public Home() : base(RoleType.Home)
    {
    }

    protected override async Task AfterCreate()
    {
        await base.AfterCreate();
        StartPlayerShardProxy();
    }
}