using Base.State;

namespace Base;

public abstract class IPlayerComponent<T> : IActorComponent where T : BaseState
{
    //数据
    public T State;

    public IPlayerComponent(BaseActor a) : base(a)
    {
    }

    public Type GetStateType => typeof(T);
}