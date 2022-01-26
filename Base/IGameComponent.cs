namespace Base;

public abstract class IGameComponent : IActorComponent
{
    public IGameComponent(BaseActor a) : base(a)
    {
    }
}