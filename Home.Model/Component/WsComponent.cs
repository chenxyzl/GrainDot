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
        public WsComponent() { }

        public Task Load()
        {
            return Task.CompletedTask;
        }


        public async Task Start()
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

        public Task PreStop()
        {
            _server.Close();
            return Task.CompletedTask;
        }
        public Task Stop()
        {
            return Task.CompletedTask;
        }

        public Task Tick()
        {
            //tick里检查所有的链接是否有超时未登陆的 如果有则关闭链接
            return Task.CompletedTask;
        }
    }
}
