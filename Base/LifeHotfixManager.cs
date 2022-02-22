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
    private Dictionary<ulong, Dictionary<Type, LoadDelegate>> _loadDelegateDic = new();
    private Dictionary<ulong, Dictionary<Type, StartDelegate>> _startDelegateDic = new();
    private Dictionary<ulong, Dictionary<Type, PreStopDelegate>> _preStopDelegateDic = new();
    private Dictionary<ulong, Dictionary<Type, StopDelegate>> _stopDelegateDic = new();
    private Dictionary<ulong, Dictionary<Type, TickDelegate>> _tickDelegateDic = new();

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
        if (!_loadDelegateDic.ContainsKey(id)) _loadDelegateDic.TryAdd(id, new());
        if (!_loadDelegateDic.TryGetValue(id, out var dic))
        {
            dic = new();
            _loadDelegateDic.TryAdd(id, dic);
        }

        var type = t.GetType();
        if (!dic.TryGetValue(type, out var loadDelegate))
        {
            _loadMethodDic.TryGetValue(type, out var temp);
            var method = A.NotNull(temp, des: type.Name + ": can found load method");
            var del = (LoadDelegate) method.CreateDelegate(typeof(LoadDelegate), t);
            loadDelegate = A.NotNull(del, des: type.Name + ": can create delegate");
            dic.TryAdd(type, loadDelegate);
        }

        return loadDelegate;
    }

    public StartDelegate GetStartDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (!_startDelegateDic.ContainsKey(id)) _startDelegateDic.TryAdd(id, new());
        if (!_startDelegateDic.TryGetValue(id, out var dic))
        {
            dic = new();
            _startDelegateDic.TryAdd(id, dic);
        }

        var type = t.GetType();
        if (!dic.TryGetValue(type, out var startDelegate))
        {
            _startMethodDic.TryGetValue(type, out var temp);
            var method = A.NotNull(temp, des: type.Name + ": can found start method");
            var del = (StartDelegate) method.CreateDelegate(typeof(StartDelegate), t);
            startDelegate = A.NotNull(del, des: type.Name + ": can create delegate");
            dic.TryAdd(type, startDelegate);
        }

        return startDelegate;
    }

    public PreStopDelegate GetPreStopDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (!_preStopDelegateDic.ContainsKey(id)) _preStopDelegateDic.TryAdd(id, new());
        if (!_preStopDelegateDic.TryGetValue(id, out var dic))
        {
            dic = new();
            _preStopDelegateDic.TryAdd(id, dic);
        }

        var type = t.GetType();
        if (!dic.TryGetValue(type, out var preStopDelegate))
        {
            _preStopMethodDic.TryGetValue(type, out var temp);
            var method = A.NotNull(temp, des: type.Name + ": can found preStop method");
            var del = (PreStopDelegate) method.CreateDelegate(typeof(PreStopDelegate), t);
            preStopDelegate = A.NotNull(del, des: type.Name + ": can create delegate");
            dic.TryAdd(type, preStopDelegate);
        }

        return preStopDelegate;
    }

    public StopDelegate GetStopDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (!_stopDelegateDic.ContainsKey(id)) _stopDelegateDic.TryAdd(id, new());
        if (!_stopDelegateDic.TryGetValue(id, out var dic))
        {
            dic = new();
            _stopDelegateDic.TryAdd(id, dic);
        }

        var type = t.GetType();
        if (!dic.TryGetValue(type, out var stopDelegate))
        {
            _stopMethodDic.TryGetValue(type, out var temp);
            var method = A.NotNull(temp, des: type.Name + ": can found stop method");
            var del = (StopDelegate) method.CreateDelegate(typeof(StopDelegate), t);
            stopDelegate = A.NotNull(del, des: type.Name + ": can create delegate");
            dic.TryAdd(type, stopDelegate);
        }

        return stopDelegate;
    }

    public TickDelegate GetTickDelegate<T>(T t, ulong id) where T : IComponent
    {
        if (!_tickDelegateDic.ContainsKey(id)) _tickDelegateDic.TryAdd(id, new());
        if (!_tickDelegateDic.TryGetValue(id, out var dic))
        {
            dic = new();
            _tickDelegateDic.TryAdd(id, dic);
        }

        var type = t.GetType();
        if (!dic.TryGetValue(type, out var tickDelegate))
        {
            _tickMethodDic.TryGetValue(type, out var temp);
            var method = A.NotNull(temp, des: type.Name + ": can found tick method");
            var del = (TickDelegate) method.CreateDelegate(typeof(TickDelegate), t);
            tickDelegate = A.NotNull(del, des: type.Name + ": can create delegate");
            dic.TryAdd(type, tickDelegate);
        }

        return tickDelegate;
    }
}