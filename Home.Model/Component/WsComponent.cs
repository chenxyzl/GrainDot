using Base;
using Base.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model
{
    public class WsComponent : IGlobalComponent
    {
        IWebSocketServer _server;
        public WsComponent(GameServer n) : base(n) { }

        public override Task PreStop()
        {
            throw new NotImplementedException();
        }

        public override async Task AfterLoad()
        {
            await StartWsServer<WsPlayerChannel>(15001);
        }

        public async Task StartWsServer<T>(ushort port) where T : WebSocketConnection
        {
            _server = await SocketBuilderFactory.GetWebSocketServerBuilder<T>(6001)
                .OnException(ex =>
                {
                    Console.WriteLine($"服务端异常:{ex.Message}");
                })
                .OnServerStarted(server =>
                {
                    Console.WriteLine($"服务启动");
                }).BuildAsync();
        }
    }
}
