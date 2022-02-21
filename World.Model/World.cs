using Base;
using Common;
using Share.Model.Component;

namespace World.Model;

public class World : GameServer
{
    public new static World Instance => A.NotNull(_ins as World);

    public World() : base(RoleType.World)
    {
    }

    protected override void RegisterComponent()
    {
        GameServer.Instance.AddComponent<DBComponent>("mongodb://admin:Qwert123!@10.7.69.214:27017");
    }
}