using Base;
using Home.Model.State;
using Share.Model.Component;

namespace Home.Model.Component;

public class MailComponent : IPlayerComponent<MailState, PlayerActor>
{
    public MailComponent(PlayerActor a) : base(a)
    {
    }
}