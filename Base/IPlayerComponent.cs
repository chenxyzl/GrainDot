using System.Threading.Tasks;
using Base.Player;
using Base.State;

namespace Base;

public abstract class IPlayerComponent<T, NT> : IActorComponent<NT> where T : BaseState where NT : BaseActor
{
    //数据
    public T? State = null!;

    public IPlayerComponent(NT a) : base(a)
    {
    }

    public Type GetStateType => typeof(T);
}