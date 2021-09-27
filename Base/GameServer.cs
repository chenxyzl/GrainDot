using Akka.Actor;
using Akka.Configuration;
using Base.Alg;
using Base.Network;
using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    [Server]
    public abstract class GameServer
    {
        //日志
        public readonly ILog Logger;
        //配置
        protected Config _systemConfig;
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
        public K GetComponent<K>() where K : IGlobalComponent
        {
            IGlobalComponent component;
            if (!this._components.TryGetValue(typeof(K), out component))
            {
                A.Abort(Message.Code.Error, $"game component:{typeof(K).Name} not found"); ;
            }

            return (K)component;
        }

        protected void AddComponent<K>() where K : IGlobalComponent
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
            var config = File.ReadAllText($"../../Config/{role}.conf");
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
                await x.AfterLoad();
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
        virtual public async Task StartSystem()
        {
            await BeforCreate();
            system = ActorSystem.Create(GlobalParam.SystemName, _systemConfig);
            await AfterCreate();
        }

        virtual public async Task StopSystem()
        {
            await PreStop();
            await system.Terminate();
            await Stop();
        }

        public async Task<IWebSocketServer> StartWsServer<T>(ushort port) where T : WebSocketConnection
        {
            var server = await SocketBuilderFactory.GetWebSocketServerBuilder<T>(6001)
                .OnException(ex =>
                {
                    Console.WriteLine($"服务端异常:{ex.Message}");
                })
                .OnServerStarted(server =>
                {
                    Console.WriteLine($"服务启动");
                }).BuildAsync(); ;
            return server;
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
    }
}
