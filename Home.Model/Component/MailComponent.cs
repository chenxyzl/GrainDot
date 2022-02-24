using Base;
using Home.Model.State;

namespace Home.Model.Component;

public class MailComponent : IPlayerComponent<MailState, PlayerActor>
{
    public MailComponent(PlayerActor a) : base(a)
    {
    }
}