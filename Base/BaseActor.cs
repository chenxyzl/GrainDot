using System.Collections.Generic;
using Akka.Actor;
using Message;

namespace Base;

public abstract class BaseActor : UntypedActor
{
    private ICancelable _cancel;

    public abstract ILog Logger { get; }

    public IActorRef GetSelf()
    {
        return Self;
    }

    public IActorRef GetSender()
    {
        return Sender;
    }

    protected override void PostStop()
    {
        base.PostStop();

        _cancel?.Cancel();
        _cancel = null;
    }


    protected void EnterUpState()
    {
        if (_cancel == null)
            _cancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.Zero,
                TimeSpan.FromMilliseconds(1), Self, new TickT(), Self);
    }

    public void ElegantStop()
    {
        Context.Parent.Tell(PoisonPill.Instance, Self);
    }

    public IActorContext GetContext()
    {
        return Context;
    }

    public class TickT
    {
    }

    #region 全局组件i

    //所有model
    public Dictionary<Type, IComponent> _components = new();

    public List<IComponent> _componentsList = new();

    //获取所有组件
    public Dictionary<Type, IComponent> GetAllComponent()
    {
        return _components;
    }

    //获取model
    public K GetComponent<K>() where K : class, IComponent
    {
        IComponent component;
        if (!_components.TryGetValue(typeof(K), out component))
        {
            A.Abort(Code.Error, $"actor component:{typeof(K).Name} not found");
            ;
        }

        return (K) component;
    }

    public void AddComponent<K>(params object[] args) where K : class, IComponent
    {
        IComponent component;
        var t = typeof(K);
        if (_components.TryGetValue(t, out component)) A.Abort(Code.Error, $"actor component:{t.Name} repeated");

        var allArgs = new List<object>();
        foreach (var a in args) allArgs.Add(a);
        var obj = Activator.CreateInstance(t, allArgs.ToArray()) as K;
        _components.Add(t, obj);
        _componentsList.Add(obj);
    }

    #endregion
}