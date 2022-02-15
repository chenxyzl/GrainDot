namespace Base;

public abstract class IActorComponent<NT> : IComponent where NT : BaseActor
{
    public NT Node;

    public IActorComponent(NT a)
    {
        Node = a;
    }
}