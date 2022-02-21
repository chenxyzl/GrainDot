namespace Base;

public interface IComponent
{
    public LoadDelegate Load => LifeHotfixManager.Instance.GetLoadDelegate(GetType());
    public StartDelegate Start => LifeHotfixManager.Instance.GetStartDelegate(GetType());
    public PreStopDelegate PreStop => LifeHotfixManager.Instance.GetPreStopDelegate(GetType());
    public StopDelegate Stop => LifeHotfixManager.Instance.GetStopDelegate(GetType());
    public TickDelegate Tick => LifeHotfixManager.Instance.GetTickDelegate(GetType());
}