namespace Base;

public abstract class IActorComponent : IComponent
{
    public BaseActor Node;

    public IActorComponent(BaseActor a)
    {
        Node = a;
    }
}