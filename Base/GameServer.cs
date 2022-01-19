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

namespace Base
{
    [Server]
    public abstract class GameServer
    {
        //日志
        public readonly ILog Logger;
        //配置
        protected Akka.Configuration.Config _systemConfig;
        //根系统
        public ActorSystem system { get; protected set; }
        //角色类型
        public RoleDef role { get; private set; }
        public GameServer(RoleDef r)
        {
            role = r;
            Logger = new NLogAdapter(role.ToString());
        }
        #region 全局组件
        //所有model
        public Dictionary<Type, IGlobalComponent> _components = new Dictionary<Type, IGlobalComponent>();
        public List<IGlobalComponent> _componentsList = new List<IGlobalComponent>();
        //获取model
        public K GetComponent<K>() where K : class, IGlobalComponent
        {
            IGlobalComponent component;
            if (!this._components.TryGetValue(typeof(K), out component))
            {
                A.Abort(Message.Code.Error, $"game component:{typeof(K).Name} not found"); ;
            }

            return (K)component;
        }

        protected void AddComponent<K>() where K : class, IGlobalComponent
        {
            IGlobalComponent component;
            Type t = typeof(K);
            if (this._components.TryGetValue(t, out component))
            {
                A.Abort(Message.Code.Error, $"game component:{t.Name} repeated");
            }
            var arg = new object[] { this };
            K obj = Activator.CreateInstance(t, arg) as K;
            _components.Add(t, obj);
            _componentsList.Add(obj);
        }
        #endregion

        private void LoadConfig()
        {
            var config = File.ReadAllText($"../Conf/{role}.conf");
            _systemConfig = ConfigurationFactory.ParseString(config);
        }

        virtual protected async Task BeforCreate()
        {
            RegisterGlobalComponent();
            LoadConfig();
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
        }


        public async Task Tick()
        {
            //全局触发PreStop
            foreach (var x in _componentsList)
            {
                await x.Tick();
            }
        }

        protected async Task PreStop()
        {
            //全局触发PreStop
            foreach (var x in _componentsList)
            {
                await x.PreStop();
            }
        }

        protected async Task Stop()
        {
            //全局触发PreStop
            foreach (var x in _componentsList)
            {
                await x.Stop();
            }
        }
        virtual public async Task StartSystem(string typeName, Props p, HashCodeMessageExtractor extractor)
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

        virtual public async Task StopSystem()
        {
            await PreStop();
            await system.Terminate();
            await Stop();
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

        /// <summary>
        /// 注册全局组件
        /// </summary>
        public abstract void RegisterGlobalComponent();

        public void StartPlayerShardProxy()
        {
            ClusterSharding.Get(system).StartProxy(GameSharedRole.Player.ToString(), role.ToString(), MessageExtractor.PlayerMessageExtractor);
        }
        public void StartWorldShardProxy()
        {
            ClusterSharding.Get(system).StartProxy(GameSharedRole.World.ToString(), role.ToString(), MessageExtractor.WorldMessageExtractor);
        }
    }
}
