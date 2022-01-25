namespace Base;

public interface IActorState
{
    void Tick(long now);
    void HandleMsg(object message);
}