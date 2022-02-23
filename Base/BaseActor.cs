using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch;
using Base.Helper;
using Message;

namespace Base;

public abstract class BaseActor : UntypedActor, IWithTimers
{
    private ICancelable? _cancel;
    public ulong uid;

    public bool LoadComplete { get; private set; }

    public abstract ActorLog Logger { get; }
    public ITimerScheduler? Timers { get; set; }

    public IActorRef GetSelf()
    {
        return Context.Self;
    }

    public IActorRef GetSender()
    {
        return Sender;
    }


    private async Task Load()
    {
        var components = GetAllComponent();
        foreach (var component in components)
        {
            await component.Load();
        }
    }

    private async Task Start()
    {
        var components = GetAllComponent();
        foreach (var component in components)
        {
            await component.Start();
        }
    }

    private async Task PreStop()
    {
        
        var components = GetAllComponent();
        foreach (var component in components)
        {
            await component.PreStop();
        }
    }

    private async Task Stop()
    {
        var components = GetAllComponent();
        foreach (var component in components)
        {
            await component.Stop();
        }
    }

    protected async Task Tick(long now)
    {
        var components = GetAllComponent();
        foreach (var component in components)
        {
            await component.Tick(now);
        }
    }

    protected override void PreStart()
    {
        ActorTaskScheduler.RunTask(
            async () =>
            {
                base.PreStart();
                await Load();
                base.PreStart();
                await Start();
                EnterUpState();

                _cancel ??= Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.Zero,
                    TimeSpan.FromSeconds(30), Self, new TickT(), Self);
            }
        );
    }

    protected override void PostStop()
    {
        ActorTaskScheduler.RunTask(
            async () =>
            {
                _cancel?.Cancel();
                _cancel = null;
                await PreStop();
                base.PostStop();
                await Stop();
            }
        );
    }

    protected virtual void EnterUpState()
    {
        LoadComplete = true;
        // Timers.StartSingleTimer(1, message, TimeSpan.FromSeconds(600));
    }

    protected void ElegantStop()
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

    #region 全局组件

    //所有model
    public Dictionary<Type, IComponent> _components = new();

    public List<IComponent> _componentsList = new();

    //获取所有组件
    public List<IComponent> GetAllComponent()
    {
        return _componentsList;
    }

    //获取model
    public C GetComponent<C>() where C : class, IComponent
    {
        _components.TryGetValue(typeof(C), out var component);
        component = A.NotNull(component, Code.Error, $"actor component:{typeof(C).Name} not found");
        return (C) component;
    }

    public void AddComponent<C>(params object[] args) where C : class, IComponent
    {
        var t = typeof(C);
        if (_components.TryGetValue(t, out _)) A.Abort(Code.Error, $"actor component:{t.Name} repeated");

        var allArgs = new List<object> {this};
        foreach (var a in args) allArgs.Add(a);
        C obj = A.NotNull(Activator.CreateInstance(t, allArgs.ToArray()) as C);
        _components.Add(t, obj);
        _componentsList.Add(obj);
    }

    #endregion
}