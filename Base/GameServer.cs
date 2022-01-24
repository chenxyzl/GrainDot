using Akka.Actor;
using Akka.Configuration;
using Common;
using System.Collections.Generic;
using System.IO;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using System.Threading.Tasks;
using Akka.Cluster.Tools.Client;
using System;
using Base.Helper;
using System.Threading;
using System.Text;

namespace Base
{
    [Server]
    public abstract class GameServer
    {
        //
        static public GameServer Instance;
        //日志
        public readonly ILog Logger;
        //配置
        protected Akka.Configuration.Config _systemConfig;
        //根系统
        public ActorSystem system { get; protected set; }
        //角色类型
        public RoleType role { get; private set; }
        //退出标记
        bool _quitFlag = false;
        long lastTime = 0;

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
        //
        public GameServer(RoleType r)
        {
            role = r;
            Logger = new NLogAdapter(role.ToString());
        }
        #region 全局组件
        //所有model
        protected Dictionary<Type, IGlobalComponent> _components = new Dictionary<Type, IGlobalComponent>();
        protected List<IGlobalComponent> _componentsList = new List<IGlobalComponent>();
        //获取model
        public K GetComponent<K>() where K : class, IGlobalComponent
        {
            if (!this._components.TryGetValue(typeof(K), out var component))
            {
                A.Abort(Message.Code.Error, $"game component:{typeof(K).Name} not found"); ;
            }

            return (K)component;
        }

        protected void AddComponent<K>(params object[] args) where K : class, IGlobalComponent
        {
            Type t = typeof(K);
            if (this._components.TryGetValue(t, out var _))
            {
                A.Abort(Message.Code.Error, $"game component:{t.Name} repeated");
            }
            K obj = Activator.CreateInstance(t, args) as K;
            _components.Add(t, obj);
            _componentsList.Add(obj);
        }
        #endregion

        virtual protected void LoadConfig()
        {
            var config = File.ReadAllText($"../Conf/{role}.conf");
            _systemConfig = ConfigurationFactory.ParseString(config);
        }

        virtual protected async Task BeforCreate()
        {
            //拦截退出
            WatchQuit();
            //加载配置
            LoadConfig();
            //注册组建
            RegisterGlobalComponent();
            //全局触发load
            foreach (var x in _componentsList)
            {
                await x.Load();
            }
        }

        virtual protected async Task AfterCreate()
        {
            //全局触发AfterLoad
            foreach (var x in _componentsList)
            {
                await x.Start();
            }
            //触发挤时间
            Instance.lastTime = TimeHelper.Now();
        }


        virtual protected async Task Tick()
        {
            //全局触发PreStop
            foreach (var x in _componentsList)
            {
                await x.Tick();
            }
        }

        virtual protected async Task PreStop()
        {
            //全局触发PreStop
            foreach (var x in _componentsList)
            {
                await x.PreStop();
            }
        }

        virtual protected async Task Stop()
        {
            //全局触发PreStop
            foreach (var x in _componentsList)
            {
                await x.Stop();
            }
        }

        virtual protected async Task StartSystem(string typeName, Props p, HashCodeMessageExtractor extractor)
        {
            await BeforCreate();
            system = ActorSystem.Create(GlobalParam.SystemName, _systemConfig);
            var sharding = ClusterSharding.Get(system);
            var shardRegion = await sharding.StartAsync(
                typeName,
                p,
                ClusterShardingSettings.Create(system),
                extractor
                );
            ClusterClientReceptionist.Get(system).RegisterService(shardRegion);
            await AfterCreate();
        }

        virtual protected async Task StartSystem()
        {
            await BeforCreate();
            system = ActorSystem.Create(GlobalParam.SystemName, _systemConfig);
            await AfterCreate();
        }

        virtual protected async Task StopSystem()
        {

            GlobalLog.Warning($"---{role}停止中,请勿强关---");
            await PreStop();
            await system.Terminate();
            await Stop();
            GlobalLog.Warning($"---{role}停止完成---");
        }

        public IActorRef GetChild(string path)
        {
            var a = system.ActorSelection(path);
            if (a == null)
            {
                A.Abort(Message.Code.Error, $"local system get child path:{path} not found");
            }
            return a.Anchor;
        }

        //加载程序集合
        virtual protected void Reload()
        {
            GlobalLog.Warning($"---{role}加载中---");
            HotfixManager.Instance.Reload();
            GlobalLog.Warning($"---{role}加载完成---");
        }

        virtual protected void Loop()
        {
            GlobalLog.Warning($"---{role}开启loop---");
            while (!_quitFlag)
            {
                Thread.Sleep(1);
                var now = TimeHelper.Now();
                //1000毫秒tick一次
                if (now - lastTime < 1000) continue;
                lastTime += 1000;
                _ = Tick();
            }
            GlobalLog.Warning($"---{role}退出loop---");
        }

        static private void BeforeRun()
        {
            //支持gbk2132
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        //有actor的启动
        static public async Task Run(Type gsType, string typeName, Props p, HashCodeMessageExtractor extractor)
        {
            //before
            BeforeRun();
            //创建
            Instance = Activator.CreateInstance(gsType) as GameServer;
            //准备
            Instance.Reload();
            //开始游戏
            await Instance.StartSystem(typeName, p, extractor);
            //开启无限循环
            Instance.Loop();
            //结束游戏
            await Instance.StopSystem();
        }
        //无actor的启动
        static public async Task Run(Type gsType)
        {
            //before；
            BeforeRun();
            //创建
            Instance = Activator.CreateInstance(gsType) as GameServer;
            //准备
            Instance.Reload();
            //开始游戏
            await Instance.StartSystem();
            //开启无限循环
            Instance.Loop();
            //结束游戏
            await Instance.StopSystem();
        }


        /// <summary>
        /// 注册全局组件
        /// </summary>
        public abstract void RegisterGlobalComponent();


        #region 开启各种proxy
        virtual protected void StartPlayerShardProxy()
        {
            ClusterSharding.Get(system).StartProxy(GameSharedRole.Player.ToString(), role.ToString(), MessageExtractor.PlayerMessageExtractor);
        }
        virtual protected void StartWorldShardProxy()
        {
            ClusterSharding.Get(system).StartProxy(GameSharedRole.World.ToString(), role.ToString(), MessageExtractor.WorldMessageExtractor);
        }
        #endregion
    }
}
