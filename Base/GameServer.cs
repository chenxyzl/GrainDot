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
        private Config systemConfig;
        //根系统
        public ActorSystem system { get; private set; }
        //角色类型
        public RoleDef role { get; private set; }
        public GameServer(RoleDef r)
        {
            role = r;
            logger = new NLogAdapter(role.ToString());
        }
        public void test()
        {
            logger.Warning("xxxxx");
        }

        //启动
        public async Task Boot()
        {
            await BeforCreate();
            await AfterCreate();
        }

        private void ConfigCluster()
        {
            var config = File.ReadAllText("app.conf");
            systemConfig = ConfigurationFactory.ParseString(config);
        }

        //
        public void LoadConfig()
        {

        }

        public Task BeforCreate()
        {
            return Task.CompletedTask;
        }

        public Task CreateActorSystem()
        {
            //system = ActorSystem.Create(actorSystemName, systemConfig);
            return Task.CompletedTask;
        }

        public Task AfterCreate()
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


        public Task StartGame()
        {
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
