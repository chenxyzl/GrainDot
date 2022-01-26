using Base.State;

namespace Base;

public abstract class IPlayerComponent<T> : IActorComponent where T : BaseState
{
    //数据
    protected T _state;
    public Type GetStateType => typeof(T);

    public IPlayerComponent(BaseActor a) : base(a)
    {
    }
}