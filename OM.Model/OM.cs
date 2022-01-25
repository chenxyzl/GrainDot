using Base;
using Common;

namespace OM.Model;

public class OM : GameServer
{
    public OM() : base(RoleType.OM)
    {
    }

    public override void RegisterGlobalComponent()
    {
    }
}