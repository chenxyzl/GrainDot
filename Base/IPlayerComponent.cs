using Base.State;

namespace Base;

public abstract class IPlayerComponent<T, A> : IActorComponent<A> where T : BaseState where A : BaseActor
{
    //数据
    public T State;

    public IPlayerComponent(A a) : base(a)
    {
    }

    public Type GetStateType => typeof(T);
}