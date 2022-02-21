using Base;
using Home.Model.State;
using Share.Model.Component;

namespace Home.Model.Component;

public class BagComponent : IPlayerComponent<BagState, PlayerActor>
{
    public BagComponent(PlayerActor a) : base(a)
    {
    }
}