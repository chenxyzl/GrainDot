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
        //这里解析所有的Delegate
        var types = HotfixManager.Instance.GetTypes<ServiceAttribute>();
        foreach (var type in types)
        {
            var HostType = A.NotNull(type.GetCustomAttribute<ServiceAttribute>(),
                des: $"type:{type.Name} not found ServiceAttribute").HostType;
            GlobalLog.Debug($"check type:{type.Name} hostType:{HostType.Name} life");
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            LoadDelegate? loadDelegate = null;
            StartDelegate? startDelegate = null;
            PreStopDelegate? preStopDelegate = null;
            StopDelegate? stopDelegate = null;
            TickDelegate? tickDelegate = null;
            foreach (var method in methods)
            {
                if (!(method.IsDefined(typeof(ExtensionAttribute), false) &&
                      method.GetParameters()[0].ParameterType.IsAssignableFrom(HostType))) continue;


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

            loadDelegateTempDic[HostType] =
                A.NotNull(loadDelegate, des: $"HostType:{HostType.Name} not found loadDelegate");
            startDelegateTempDic[HostType] =
                A.NotNull(startDelegate, des: $"HostType:{HostType.Name} not found startDelegate");
            preStopDelegateTempDic[HostType] =
                A.NotNull(preStopDelegate, des: $"HostType:{HostType.Name} not found preStopDelegate");
            stopDelegateTempDic[HostType] =
                A.NotNull(stopDelegate, des: $"HostType:{HostType.Name} not found stopDelegate");
            tickDelegateTempDic[HostType] =
                A.NotNull(tickDelegate, des: $"HostType:{HostType.Name} not found tickDelegate");
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