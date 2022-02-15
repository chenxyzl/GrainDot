using Base;
using Common;

namespace World.Model;

public class World : GameServer
{
    public new static World Instance => A.NotNull(_ins as World);

    public World() : base(RoleType.World)
    {
    }
}