using Base;
using Home.Model.State;

namespace Home.Model.Component;

public class BagComponent : IPlayerComponent<BagState>
{
    public BagComponent(BaseActor a) : base(a)
    {
    }
}