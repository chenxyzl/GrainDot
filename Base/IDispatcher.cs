namespace Base;

public interface IComponent
{
    public LoadDelegate Load { get; }
    public StartDelegate Start { get; }
    public PreStopDelegate PreStop { get; }
    public StopDelegate Stop { get; }
    public TickDelegate Tick { get; }
}