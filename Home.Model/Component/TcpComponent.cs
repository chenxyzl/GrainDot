using Base;
using Base.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model
{
    public class TcpComponent : IGlobalComponent
    {
        private ITcpSocketServer _server;
        public TcpComponent(GameServer n) : base(n) { }
        public override Task Load()
        {
            return Task.CompletedTask;
        }

        public override async Task Start(bool crossDay)
        {
            _server = await StartTcpServer<PlayerChannel>(1000);
        }

        public override Task PreStop()
        {
            _server.Close();
            return Task.CompletedTask;
        }

        private async Task<ITcpSocketServer> StartTcpServer<T>(ushort port) where T : TcpSocketConnection
        {
            var server = await SocketBuilderFactory.GetTcpSocketServerBuilder<T>(6001)
                .SetLengthFieldEncoder(2)
                .SetLengthFieldDecoder(ushort.MaxValue, 0, 2, 0, 2)
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
    }
}
