namespace Base;

public abstract class IActorComponent<T> : IComponent where T : BaseActor
{
    public LoadDelegate Load => LifeHotfixManager.Instance.GetLoadDelegate(this, Node.uid);
    public StartDelegate Start => LifeHotfixManager.Instance.GetStartDelegate(this, Node.uid);
    public PreStopDelegate PreStop => LifeHotfixManager.Instance.GetPreStopDelegate(this, Node.uid);
    public StopDelegate Stop => LifeHotfixManager.Instance.GetStopDelegate(this, Node.uid);
    public TickDelegate Tick => LifeHotfixManager.Instance.GetTickDelegate(this, Node.uid);

    public T Node;

    public IActorComponent(T a)
    {
        Node = a;
    }
}