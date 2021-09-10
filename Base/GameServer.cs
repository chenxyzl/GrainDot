using Akka.Actor;
using Akka.Configuration;
using Base.Alg;
using Base.Network.Server;
using Base.Network.Server.Interfaces;
using Base.Network.Shared;
using Base.Network.Shared.Interfaces;
using Common;
using System;
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
        public readonly ILog logger;
        //服务器列表
        readonly List<Server> serverList = new List<Server>();
        //配置
        protected Config systemConfig;
        //根系统
        public ActorSystem system { get; protected set; }
        //角色类型
        public RoleDef role { get; private set; }
        public GameServer(RoleDef r)
        {
            role = r;
            logger = new NLogAdapter(role.ToString());
        }

        private void LoadConfig()
        {
            var config = File.ReadAllText($"../../Config/{role}.conf");
            systemConfig = ConfigurationFactory.ParseString(config);
        }


        virtual public Task BeforCreate()
        {
            LoadConfig();
            return Task.CompletedTask;
        }

        virtual public Task CreateActorSystem()
        {
            system = ActorSystem.Create(GlobalParam.SystemName, systemConfig);
            return Task.CompletedTask;
        }

        virtual public Task AfterCreate()
        {
            return Task.CompletedTask;
        }

        public Task BeforeStop()
        {
            foreach (var x in serverList)
            {
                x.Stop();
            }
            return Task.CompletedTask;
        }

        public void StartTcpServer(ushort port)
        {
            var server = new Server(port: port);
            server.Start(NetworkListenerType.TCP);
        }
        public void StartWsServer(ushort port)
        {
            var server = new Server(port: port);
            server.Start(NetworkListenerType.WSBinary);

        }
        public void StartUdpServer(ushort port)
        {
            var server = new Server(port: port);
            server.Start(NetworkListenerType.UDP);
        }
    }
}
