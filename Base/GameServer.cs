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
    protected Akka.Configuration.Config _systemConfig = null!;

    //世界代理
    private IActorRef? _worldShardProxy;

    private long lastTime;

    //
    public GameServer(RoleType r)
    {
        role = r;
        Logger = new NLogAdapter(role.ToString());
    }

    public static GameServer Instance => A.NotNull(_ins);

    //角色类型
    public RoleType role { get; }

    //根系统
    public ActorSystem system { get; protected set; } = null!;
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
        Console.CancelKeyPress += (sender, e) =>
        {
            if (_quitFlag) return;
            _quitFlag = true;
            e.Cancel = true;
        };
    }

    protected virtual void LoadConfig()
    {
        var baseConfig = File.ReadAllText("../Conf/Base.conf");
        var config = File.ReadAllText($"../Conf/{role}.conf");
        // var o = ConfigurationFactory.Default();
        var a = ConfigurationFactory.ParseString(baseConfig);
        var b = ConfigurationFactory.ParseString(config);
        _systemConfig = b.WithFallback(a);
        //_systemConfig  = File.ReadAllText($"../Conf/{role}.conf");
    }

    protected virtual async Task BeforCreate()
    {
        Logger.Info("Register begin!!!");
        GlobalHotfixManager.Instance.Hotfix.RegisterComponent();
        Logger.Info("Register success!!!");
        //拦截退出
        WatchQuit();
        //加载配置
        LoadConfig();
        //注册mongo的State
        MongoHelper.Init();
        //全局触发load
        Logger.Info("Load begin!!!");
        await GlobalHotfixManager.Instance.Hotfix.Load();
        Logger.Info("Load success!!!");
    }

    protected virtual async Task AfterCreate()
    {
        //触发挤时间
        Instance.lastTime = TimeHelper.Now();
        Logger.Info("Start begin!!!");
        //全局触发AfterLoad
        await GlobalHotfixManager.Instance.Hotfix.Start();
        Logger.Info("Start success!!!");
    }


    protected virtual async Task Tick()
    {
        //全局触发PreStop
        await GlobalHotfixManager.Instance.Hotfix.Tick();
    }

    protected virtual async Task PreStop()
    {
        Logger.Info("preStoop begin!!!");
        //全局触发PreStop
        await GlobalHotfixManager.Instance.Hotfix.PreStop();
        Logger.Info("preStoop success!!!");
    }

    protected virtual async Task Stop()
    {
        Logger.Info("Stop begin!!!");
        //全局触发PreStop
        await GlobalHotfixManager.Instance.Hotfix.Stop();
        Logger.Info("Stop success!!!");
    }

    protected virtual async Task StartSystem(RoleType roleType, GameSharedType sharedType, Props p,
        HashCodeMessageExtractor extractor)
    {
        await BeforCreate();
        system = ActorSystem.Create(GlobalParam.SystemName, _systemConfig);
        var sharding = ClusterSharding.Get(system);
        var shardRegion = await sharding.StartAsync(
            sharedType.ToString(),
            p,
            ClusterShardingSettings.Create(system).WithRole(roleType.ToString()),
            extractor
        );
        ClusterClientReceptionist.Get(system).RegisterService(shardRegion);
        await AfterCreate();
    }

    protected virtual async Task StartSystem()
    {
        await BeforCreate();
        system = ActorSystem.Create(GlobalParam.SystemName, _systemConfig);
        await AfterCreate();
    }

    protected virtual async Task StopSystem()
    {
        GlobalLog.Warning($"---{role}停止中,请勿强关---");
        await PreStop();
        await system.Terminate();
        // ClusterClientReceptionist.Get(system).UnregisterService(Self);
        await Stop();
        GlobalLog.Warning($"---{role}停止完成---");
    }

    //加载程序集合
    protected virtual void Load(uint nodeId)
    {
        GlobalLog.Warning($"---{role}加载中---");
        IdGenerater.GlobalInit(nodeId);
        HotfixManager.Instance.Reload();
        ConfigManager.Instance.ReloadConfig();
        GlobalLog.Warning($"---{role}加载完成---");
    }

    protected virtual void Loop()
    {
        GlobalLog.Warning($"---{role}开启loop---");
        //异步时间回调到主线程
        // SynchronizationContext.SetSynchronizationContext(GlobalThreadSynchronizationContext.Instance);
        while (!_quitFlag)
        {
            // GlobalThreadSynchronizationContext.Instance.Update();
            Thread.Sleep(1);
            //1000毫秒tick一次
            var now = TimeHelper.Now();
            if (now - lastTime < 1000) continue;
            lastTime += 1000;
            _ = Tick();
        }

        GlobalLog.Warning($"---{role}退出loop---");
    }

    private static void BeforeRun()
    {
        //支持gbk2132
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    //有actor的启动
    public static async Task Run(Type gsType, GameSharedType sharedType, Props p,
        HashCodeMessageExtractor extractor, uint nodeId)
    {
        //before
        BeforeRun();
        //创建
        _ins = A.NotNull(Activator.CreateInstance(gsType) as GameServer);
        //准备
        Instance.Load(nodeId);
        //开始游戏
        await Instance.StartSystem(Instance.role, sharedType, p, extractor);
        //开启无限循环
        Instance.Loop();
        //结束游戏
        await Instance.StopSystem();
    }

    //无actor的启动
    public static async Task Run(Type gsType, uint nodeId)
    {
        //before；
        BeforeRun();
        //创建
        _ins = A.NotNull(Activator.CreateInstance(gsType) as GameServer);
        //准备
        Instance.Load(nodeId);
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
    public K GetComponent<K>() where K : class, IGlobalComponent
    {
        _components.TryGetValue(typeof(K), out var component);
        component = A.NotNull(component, Code.Error, $"game component:{typeof(K).Name} not found");
        return (K) component;
    }

    public void AddComponent<K>(params object[] args) where K : class, IGlobalComponent
    {
        var t = typeof(K);
        if (_components.TryGetValue(t, out _)) A.Abort(Code.Error, $"game component:{t.Name} repeated");

        var obj = A.NotNull(Activator.CreateInstance(t, args) as K);
        _components.Add(t, obj);
        _componentsList.Add(obj);
    }

    #endregion


    #region 开启各种proxy

    protected virtual void StartPlayerShardProxy()
    {
        ClusterSharding.Get(system).StartProxy(GameSharedType.Player.ToString(), role.ToString(),
            MessageExtractor.PlayerMessageExtractor);
        _playerShardProxy = ClusterSharding.Get(system)
            .ShardRegion(GameSharedType.Player.ToString());
    }

    protected virtual void StartWorldShardProxy()
    {
        ClusterSharding.Get(system).StartProxy(GameSharedType.World.ToString(), role.ToString(),
            MessageExtractor.WorldMessageExtractor);
        _worldShardProxy = ClusterSharding.Get(system)
            .ShardRegion(GameSharedType.World.ToString());
    }

    #endregion
}