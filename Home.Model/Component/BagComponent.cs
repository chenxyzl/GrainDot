using Base;
using Home.Model.State;

namespace Home.Model.Component;

public class BagComponent : IPlayerComponent<BagState, PlayerActor>
{
    public BagComponent(PlayerActor a) : base(a)
    {
    }
}