using Base.State;

namespace Base;

public abstract class IGameComponent<T, A> : IActorComponent<A> where T : BaseState where A : BaseActor
{
    //数据
    public T State;

    public IGameComponent(A a) : base(a)
    {
    }
}