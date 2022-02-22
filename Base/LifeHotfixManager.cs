using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Base.Helper;
using Base.Player;
using Message;

namespace Base;

public class LifeHotfixManager : Single<LifeHotfixManager>
{
    private Dictionary<ulong, LoadDelegate> _loadDelegateDic = new();
    private Dictionary<ulong, StartDelegate> _startDelegateDic = new();
    private Dictionary<ulong, PreStopDelegate> _preStopDelegateDic = new();
    private Dictionary<ulong, StopDelegate> _stopDelegateDic = new();
    private Dictionary<ulong, TickDelegate> _tickDelegateDic = new();

    private Dictionary<Type, MethodInfo> _loadMethodDic = new();
    private Dictionary<Type, MethodInfo> _startMethodDic = new();
    private Dictionary<Type, MethodInfo> _preStopMethodDic = new();
    private Dictionary<Type, MethodInfo> _stopMethodDic = new();
    private Dictionary<Type, MethodInfo> _tickMethodDic = new();

    public void ReloadHandler()
    {
        GlobalLog.Warning("life delegate reload begin");
        Dictionary<Type, MethodInfo> loadMethodTempDic = new();
        Dictionary<Type, MethodInfo> startMethodTempDic = new();
        Dictionary<Type, MethodInfo> preStopMethodTempDic = new();
        Dictionary<Type, MethodInfo> stopMethodTempDic = new();
        Dictionary<Type, MethodInfo> tickMethodTempDic = new();
        //这里解析所有的Delegate
        var types = HotfixManager.Instance.GetTypes<ServiceAttribute>();
        foreach (var type in types)
        {
            var HostType = A.NotNull(type.GetCustomAttribute<ServiceAttribute>(),
                des: $"type:{type.Name} not found ServiceAttribute").HostType;
            GlobalLog.Debug($"check type:{type.Name} hostType:{HostType.Name} life");
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            MethodInfo? loadMethod = null;
            MethodInfo? startMethod = null;
            MethodInfo? preStopMethod = null;
            MethodInfo? stopMethod = null;
            MethodInfo? tickMethod = null;
            foreach (var method in methods)
            {
                if (!(method.IsDefined(typeof(ExtensionAttribute), false) &&
                      method.GetParameters()[0].ParameterType.IsAssignableFrom(HostType))) continue;


                switch (method.Name)
                {
                    case "Load":
                    {
                        loadMethod = method;
                        _ = (LoadDelegate) method.CreateDelegate(typeof(LoadDelegate), null);
                        break;
                    }
                    case "Start":
                    {
                        startMethod = method;
                        _ = (StartDelegate) method.CreateDelegate(typeof(StartDelegate), null);
                        break;
                    }
                    case "PreStop":
                    {
                        preStopMethod = method;
                        _ = (PreStopDelegate) method.CreateDelegate(typeof(PreStopDelegate), null);
                        break;
                    }
                    case "Stop":
                    {
                        stopMethod = method;
                        _ = (StopDelegate) method.CreateDelegate(typeof(StopDelegate), null);
                        break;
                    }
                    case "Tick":
                    {
                        tickMethod = method;
                        _ = (TickDelegate) method.CreateDelegate(typeof(TickDelegate), null);
                        break;
                    }
                }
            }

            loadMethodTempDic[HostType] =
                A.NotNull(loadMethod, des: $"HostType:{HostType.Name} not found Load method");
            startMethodTempDic[HostType] =
                A.NotNull(startMethod, des: $"HostType:{HostType.Name} not found Start method");
            preStopMethodTempDic[HostType] =
                A.NotNull(preStopMethod, des: $"HostType:{HostType.Name} not found PreStop method");
            stopMethodTempDic[HostType] =
                A.NotNull(stopMethod, des: $"HostType:{HostType.Name} not found Stop method");
            tickMethodTempDic[HostType] =
                A.NotNull(tickMethod, des: $"HostType:{HostType.Name} not found Tick method");
        }

        _loadMethodDic = loadMethodTempDic;
        _startMethodDic = startMethodTempDic;
        _preStopMethodDic = preStopMethodTempDic;
        _stopMethodDic = stopMethodTempDic;
        _tickMethodDic = tickMethodTempDic;
        _loadDelegateDic = new();
        _startDelegateDic = new();
        _preStopDelegateDic = new();
        _stopDelegateDic = new();
        _tickDelegateDic = new();
        GlobalLog.Warning("life delegate reload success");
    }

    public LoadDelegate GetLoadDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (_loadDelegateDic.TryGetValue(id, out var de))
        {
            return de;
        }

        var type = t.GetType();
        _loadMethodDic.TryGetValue(type, out var temp);
        var method = A.NotNull(temp, des: type.Name + ": can found load method");
        var del = (LoadDelegate) method.CreateDelegate(typeof(LoadDelegate), t);
        var loadDelegate = A.NotNull(del, des: type.Name + ": can found load method");
        _loadDelegateDic.TryAdd(id, loadDelegate);
        return loadDelegate;
    }

    public StartDelegate GetStartDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (_startDelegateDic.TryGetValue(id, out var de))
        {
            return de;
        }

        var type = t.GetType();
        _startMethodDic.TryGetValue(type, out var temp);
        var method = A.NotNull(temp, des: type.Name + ": can found start method");
        var del = (StartDelegate) method.CreateDelegate(typeof(StartDelegate), t);
        var startDelegate = A.NotNull(del, des: type.Name + ": can found start method");
        _startDelegateDic.TryAdd(id, startDelegate);
        return startDelegate;
    }

    public PreStopDelegate GetPreStopDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (_preStopDelegateDic.TryGetValue(id, out var de))
        {
            return de;
        }

        var type = t.GetType();
        _preStopMethodDic.TryGetValue(type, out var temp);
        var method = A.NotNull(temp, des: type.Name + ": can found preStop method");
        var del = (PreStopDelegate) method.CreateDelegate(typeof(PreStopDelegate), t);
        var preStopDelegate = A.NotNull(del, des: type.Name + ": can found preStop method");
        _preStopDelegateDic.TryAdd(id, preStopDelegate);
        return preStopDelegate;
    }

    public StopDelegate GetStopDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (_stopDelegateDic.TryGetValue(id, out var de))
        {
            return de;
        }

        var type = t.GetType();
        _stopMethodDic.TryGetValue(type, out var temp);
        var method = A.NotNull(temp, des: type.Name + ": can found stop method");
        var del = (StopDelegate) method.CreateDelegate(typeof(StopDelegate), t);
        var stopDelegate = A.NotNull(del, des: type.Name + ": can found stop method");
        _stopDelegateDic.TryAdd(id, stopDelegate);
        return stopDelegate;
    }

    public TickDelegate GetTickDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (_tickDelegateDic.TryGetValue(id, out var de))
        {
            return de;
        }

        var type = t.GetType();
        _tickMethodDic.TryGetValue(type, out var temp);
        var method = A.NotNull(temp, des: type.Name + ": can found tick method");
        var del = (TickDelegate) method.CreateDelegate(typeof(TickDelegate), t);
        var tickDelegate = A.NotNull(del, des: type.Name + ": can found tick method");
        _tickDelegateDic.TryAdd(id, tickDelegate);
        return tickDelegate;
    }
}