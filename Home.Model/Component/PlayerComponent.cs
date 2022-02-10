using Base;
using Home.Model.State;

namespace Home.Model.Component;

public class PlayerComponent : IPlayerComponent<PlayerState, PlayerActor>
{
    public PlayerComponent(PlayerActor a) : base(a)
    {
    }
}