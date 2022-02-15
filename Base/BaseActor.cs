using System.Collections.Generic;
using Akka.Actor;
using Message;

namespace Base;

public abstract class BaseActor : UntypedActor, IWithTimers
{
    private ICancelable? _cancel;
    public ulong uid;

    public bool LoadComplete { get; private set; }

    public abstract ILog Logger { get; }
    public ITimerScheduler? Timers { get; set; }

    public IActorRef GetSelf()
    {
        return Context.Self;
    }

    public IActorRef GetSender()
    {
        return Sender;
    }

    protected override void PreStart()
    {
        base.PreStart();
    }

    protected override void PostStop()
    {
        base.PostStop();

        _cancel?.Cancel();
        _cancel = null;
    }


    protected void EnterUpState()
    {
        _cancel ??= Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.Zero,
            TimeSpan.FromSeconds(30), Self, new TickT(), Self);
        LoadComplete = true;
        // Timers.StartSingleTimer(1, message, TimeSpan.FromSeconds(600));
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
        _components.TryGetValue(typeof(K), out var component);
        component = A.NotNull(component, Code.Error, $"actor component:{typeof(K).Name} not found");
        return (K) component;
    }

    public void AddComponent<K>(params object[] args) where K : class, IComponent
    {
        var t = typeof(K);
        if (_components.TryGetValue(t, out _)) A.Abort(Code.Error, $"actor component:{t.Name} repeated");

        var allArgs = new List<object> {this};
        foreach (var a in args) allArgs.Add(a);
        var obj = A.NotNull(Activator.CreateInstance(t, allArgs.ToArray()) as K, Code.Error);
        _components.Add(t, obj);
        _componentsList.Add(obj);
    }

    #endregion
}