namespace Base;

public abstract class IActorComponent<T> : IComponent where T : BaseActor
{
    public T Node;

    public IActorComponent(T a)
    {
        Node = a;
    }
}