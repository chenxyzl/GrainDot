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
    private Dictionary<Type, LoadDelegate> _loadDelegateDic = new();
    private Dictionary<Type, StartDelegate> _startDelegateDic = new();
    private Dictionary<Type, PreStopDelegate> _preStopDelegateDic = new();
    private Dictionary<Type, StopDelegate> _stopDelegateDic = new();
    private Dictionary<Type, TickDelegate> _tickDelegateDic = new();

    public void ReloadHandler()
    {
        GlobalLog.Warning("life delegate reload begin");
        Dictionary<Type, LoadDelegate> loadDelegateTempDic = new();
        Dictionary<Type, StartDelegate> startDelegateTempDic = new();
        Dictionary<Type, PreStopDelegate> preStopDelegateTempDic = new();
        Dictionary<Type, StopDelegate> stopDelegateTempDic = new();
        Dictionary<Type, TickDelegate> tickDelegateTempDic = new();
        //todo 这里解析所有的Delegate
        var types = HotfixManager.Instance.GetTypes<ServiceAttribute>();
        foreach (var type in types)
        {
            //todo 获取type所属的service
            var host = A.NotNull(type.GetCustomAttribute<ServiceAttribute>(),
                des: $"type:{type.Name} not found ServiceAttribute");
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            LoadDelegate? loadDelegate = null;
            StartDelegate? startDelegate = null;
            PreStopDelegate? preStopDelegate = null;
            StopDelegate? stopDelegate = null;
            TickDelegate? tickDelegate = null;
            foreach (var method in methods)
            {
                A.Ensure(method.IsDefined(typeof(ExtensionAttribute), false) &&
                         method.GetParameters()[0].ParameterType.IsAssignableFrom(host.HostType),
                    des: $"all method must extension by host type: {host.HostType.Name} ");

                switch (method.Name)
                {
                    case "Load":
                    {
                        loadDelegate = (LoadDelegate) method.CreateDelegate(typeof(LoadDelegate), null);
                        break;
                    }
                    case "Start":
                    {
                        startDelegate = (StartDelegate) method.CreateDelegate(typeof(StartDelegate), null);
                        break;
                    }
                    case "PreStop":
                    {
                        preStopDelegate = (PreStopDelegate) method.CreateDelegate(typeof(PreStopDelegate), null);
                        break;
                    }
                    case "Stop":
                    {
                        stopDelegate = (StopDelegate) method.CreateDelegate(typeof(StopDelegate), null);
                        break;
                    }
                    case "Tick":
                    {
                        tickDelegate = (TickDelegate) method.CreateDelegate(typeof(TickDelegate), null);
                        break;
                    }
                }
            }

            loadDelegateTempDic[type] = A.NotNull(loadDelegate, des: $"type:{type.Name} not found loadDelegate");
            startDelegateTempDic[type] = A.NotNull(startDelegate, des: $"type:{type.Name} not found startDelegate");
            preStopDelegateTempDic[type] =
                A.NotNull(preStopDelegate, des: $"type:{type.Name} not found preStopDelegate");
            stopDelegateTempDic[type] = A.NotNull(stopDelegate, des: $"type:{type.Name} not found stopDelegate");
            tickDelegateTempDic[type] = A.NotNull(tickDelegate, des: $"type:{type.Name} not found tickDelegate");
        }

        _loadDelegateDic = loadDelegateTempDic;
        _startDelegateDic = startDelegateTempDic;
        _preStopDelegateDic = preStopDelegateTempDic;
        _stopDelegateDic = stopDelegateTempDic;
        _tickDelegateDic = tickDelegateTempDic;
        GlobalLog.Warning("life delegate reload success");
    }

    public LoadDelegate GetLoadDelegate(Type t)
    {
        _loadDelegateDic.TryGetValue(t, out var de);
        return A.NotNull(de, Code.Error, "load delegate not found");
    }

    public StartDelegate GetStartDelegate(Type t)
    {
        _startDelegateDic.TryGetValue(t, out var de);
        return A.NotNull(de, Code.Error, "start delegate not found");
    }

    public PreStopDelegate GetPreStopDelegate(Type t)
    {
        _preStopDelegateDic.TryGetValue(t, out var de);
        return A.NotNull(de, Code.Error, "preStop delegate not found");
    }

    public StopDelegate GetStopDelegate(Type t)
    {
        _stopDelegateDic.TryGetValue(t, out var de);
        return A.NotNull(de, Code.Error, "stop delegate not found");
    }

    public TickDelegate GetTickDelegate(Type t)
    {
        _tickDelegateDic.TryGetValue(t, out var de);
        return A.NotNull(de, Code.Error, "tick delegate not found");
    }
}