using Base;
using Common;

namespace World.Model;

public class World : GameServer
{
    public World() : base(RoleType.World)
    {
    }

    public new static World Instance => A.NotNull(_ins as World);
}