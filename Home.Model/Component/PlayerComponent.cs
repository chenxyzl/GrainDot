using Base;
using Home.Model.State;

namespace Home.Model.Component;

public class PlayerComponent : IPlayerComponent<PlayerState>
{
    public PlayerComponent(BaseActor a) : base(a)
    {
    }
}