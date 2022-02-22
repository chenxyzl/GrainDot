using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Client;
using Akka.Configuration;
using Base.Helper;
using Common;
using Message;

namespace Base;

[Server]
public abstract class GameServer
{
    //
    protected static GameServer? _ins;

    //日志
    public readonly ILog Logger;

    //玩家代理
    private IActorRef? _playerShardProxy;

    //退出标记
    private bool _quitFlag;

    //配置
    private Akka.Configuration.Config _systemConfig = null!;

    //世界代理
    private IActorRef? _worldShardProxy;

    private long _lastTime;

    //
    public GameServer(RoleType r, ushort nodeId)
    {
        Role = r;
        NodeId = nodeId;
        Logger = new NLogAdapter(Role.ToString());
        //初始化id生成器
        IdGenerater.GlobalInit(nodeId);
    }

    public static GameServer Instance => A.NotNull(_ins);

    //角色类型
    public RoleType Role { get; }

    public ushort NodeId { get; }

    //根系统
    public ActorSystem System { get; private set; } = null!;
    public IActorRef PlayerShardProxy => A.NotNull(_playerShardProxy, Code.Error, "need StartPlayerProxy");

    public IActorRef WorldShardProxy => A.NotNull(_worldShardProxy, Code.Error, "need StartWorldProxy");

    public Akka.Configuration.Config SystemConfig => _systemConfig;

    public static GameServer Instance1<T>() where T : GameServer
    {
        return A.NotNull(Instance as T);
    }


    //退出标记监听
    protected virtual void WatchQuit()
    {
        Console.CancelKeyPress += (_, e) =>
        {
            if (_quitFlag) return;
            _quitFlag = true;
            e.Cancel = true;
        };
    }

    protected virtual void LoadConfig()
    {
        var baseConfig = File.ReadAllText("../Conf/Base.conf");
        var config = File.ReadAllText($"../Conf/{Role}.conf");
        // var o = ConfigurationFactory.Default();
        var a = ConfigurationFactory.ParseString(baseConfig);
        var b = ConfigurationFactory.ParseString(config);
        _systemConfig = b.WithFallback(a);
        //_systemConfig  = File.ReadAllText($"../Conf/{role}.conf");
    }

    protected abstract void RegisterComponent();

    protected virtual async Task BeforCreate()
    {
        Logger.Info("Register begin!!!");
        RegisterComponent();
        Logger.Info("Register success!!!");
        //拦截退出
        WatchQuit();
        //加载配置
        LoadConfig();
        //注册mongo的State
        //全局触发load
        Logger.Info("Load begin!!!");
        foreach (var component in _componentsList)
        {
            await component.Load();
        }

        Logger.Info("Load success!!!");
    }

    protected virtual async Task AfterCreate()
    {
        //触发挤时间
        Instance._lastTime = TimeHelper.Now();
        Logger.Info("Start begin!!!");
        //全局触发AfterLoad
        foreach (var component in _componentsList)
        {
            await component.Start();
        }

        Logger.Info("Start success!!!");
    }


    protected virtual async Task Tick(long now)
    {
        foreach (var component in _componentsList)
        {
            await component.Tick(now);
        }
    }

    protected virtual async Task PreStop()
    {
        Logger.Info("preStoop begin!!!");
        //全局触发PreStop
        foreach (var component in _componentsList)
        {
            await component.PreStop();
        }

        Logger.Info("preStoop success!!!");
    }

    protected virtual async Task Stop()
    {
        Logger.Info("Stop begin!!!");
        //全局触发Stop
        foreach (var component in _componentsList)
        {
            await component.Stop();
        }

        Logger.Info("Stop success!!!");
    }

    protected virtual async Task StartSystem(RoleType roleType, GameSharedType sharedType, Props p,
        HashCodeMessageExtractor extractor)
    {
        await BeforCreate();
        System = ActorSystem.Create(GlobalParam.SystemName, _systemConfig);
        var sharding = ClusterSharding.Get(System);
        var shardRegion = await sharding.StartAsync(
            sharedType.ToString(),
            p,
            ClusterShardingSettings.Create(System).WithRole(roleType.ToString()),
            extractor
        );
        ClusterClientReceptionist.Get(System).RegisterService(shardRegion);
        await AfterCreate();
    }

    protected virtual async Task StartSystem()
    {
        await BeforCreate();
        System = ActorSystem.Create(GlobalParam.SystemName, _systemConfig);
        await AfterCreate();
    }

    protected virtual async Task StopSystem()
    {
        GlobalLog.Warning($"---{Role}停止中,请勿强关---");
        await PreStop();
        await System.Terminate();
        // ClusterClientReceptionist.Get(system).UnregisterService(Self);
        await Stop();
        GlobalLog.Warning($"---{Role}停止完成---");
    }

    //加载程序集合
    protected virtual void Load()
    {
        GlobalLog.Warning($"---{Role}加载中---");
        HotfixManager.Instance.Reload();
        ConfigManager.Instance.ReloadConfig();
        GlobalLog.Warning($"---{Role}加载完成---");
    }

    protected virtual void Loop()
    {
        GlobalLog.Warning($"---{Role}开启loop---");
        //异步时间回调到主线程
        // SynchronizationContext.SetSynchronizationContext(GlobalThreadSynchronizationContext.Instance);
        while (!_quitFlag)
        {
            // GlobalThreadSynchronizationContext.Instance.Update();
            Thread.Sleep(1);
            //1000毫秒tick一次
            var now = TimeHelper.Now();
            if (now - _lastTime < 1000) continue;
            _lastTime += 1000;
            _ = Tick(now);
        }

        GlobalLog.Warning($"---{Role}退出loop---");
    }

    private static void BeforeRun(Type gsType, ushort nodeId)
    {
        //支持gbk2132
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //创建
        _ins = A.NotNull(Activator.CreateInstance(gsType, nodeId) as GameServer);
    }

    //有actor的启动
    public static async Task Run(Type gsType, GameSharedType sharedType, Props p,
        HashCodeMessageExtractor extractor, ushort nodeId)
    {
        //before
        BeforeRun(gsType, nodeId);
        //准备
        Instance.Load();
        //开始游戏
        await Instance.StartSystem(Instance.Role, sharedType, p, extractor);
        //开启无限循环
        Instance.Loop();
        //结束游戏
        await Instance.StopSystem();
    }

    //无actor的启动
    public static async Task Run(Type gsType, ushort nodeId)
    {
        //before；
        BeforeRun(gsType, nodeId);
        //准备
        Instance.Load();
        //开始游戏
        await Instance.StartSystem();
        //开启无限循环
        Instance.Loop();
        //结束游戏
        await Instance.StopSystem();
    }


    #region 全局组件

    //所有model
    protected Dictionary<Type, IGlobalComponent> _components = new();

    protected List<IGlobalComponent> _componentsList = new();

    //获取model
    public C GetComponent<C>() where C : IGlobalComponent
    {
        _components.TryGetValue(typeof(C), out var component);
        component = A.NotNull(component, Code.Error, $"game component:{typeof(C).Name} not found");
        return (C) component;
    }

    public void AddComponent<C>(params object[] args) where C : IGlobalComponent
    {
        var t = typeof(C);
        if (_components.TryGetValue(t, out _)) A.Abort(Code.Error, $"game component:{t.Name} repeated");

        var obj = A.NotNull(Activator.CreateInstance(t, args) as C);
        _components.Add(t, obj);
        _componentsList.Add(obj);
    }

    #endregion


    #region 开启各种proxy

    protected virtual void StartPlayerShardProxy()
    {
        ClusterSharding.Get(System).StartProxy(GameSharedType.Player.ToString(), Role.ToString(),
            MessageExtractor.PlayerMessageExtractor);
        _playerShardProxy = ClusterSharding.Get(System)
            .ShardRegion(GameSharedType.Player.ToString());
    }

    protected virtual void StartWorldShardProxy()
    {
        ClusterSharding.Get(System).StartProxy(GameSharedType.World.ToString(), Role.ToString(),
            MessageExtractor.WorldMessageExtractor);
        _worldShardProxy = ClusterSharding.Get(System)
            .ShardRegion(GameSharedType.World.ToString());
    }

    #endregion
}