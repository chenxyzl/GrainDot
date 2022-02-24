namespace Base;

public abstract class IGlobalComponent : IComponent
{
    public LoadDelegate Load =>
        LifeHotfixManager.Instance.GetLoadDelegate(this, GameServer.Instance.NodeId);

    public StartDelegate Start =>
        LifeHotfixManager.Instance.GetStartDelegate(this, GameServer.Instance.NodeId);

    public PreStopDelegate PreStop =>
        LifeHotfixManager.Instance.GetPreStopDelegate(this, GameServer.Instance.NodeId);

    public StopDelegate Stop =>
        LifeHotfixManager.Instance.GetStopDelegate(this, GameServer.Instance.NodeId);

    public TickDelegate Tick =>
        LifeHotfixManager.Instance.GetTickDelegate(this, GameServer.Instance.NodeId);
}