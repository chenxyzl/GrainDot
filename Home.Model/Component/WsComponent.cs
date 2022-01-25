using System;
using System.Threading.Tasks;
using Base;
using Base.Network;

namespace Home.Model.Component;

public class WsComponent : IGlobalComponent
{
    private IWebSocketServer _server;

    public Task Load()
    {
        return Task.CompletedTask;
    }


    public async Task Start()
    {
        await StartWsServer<WsPlayerChannel>(15001);
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

    public async Task StartWsServer<T>(ushort port) where T : WebSocketConnection
    {
        _server = await SocketBuilderFactory.GetWebSocketServerBuilder<T>(port)
            .OnException(ex => { Console.WriteLine($"{GetType().Name}:{port} 服务端异常:{ex.Message}"); })
            .OnNewConnection((server, connection) =>
            {
                GameServer.Instance.GetComponent<ConnectionDicCommponent>().AddConnection(connection);
            })
            .OnConnectionClose(
                (server, connection) =>
                {
                    GameServer.Instance.GetComponent<ConnectionDicCommponent>()
                        .RemoveConnection(connection.ConnectionId);
                })
            .OnServerStarted(server => { Console.WriteLine($"{GetType().Name}:{port} 服务启动"); }).BuildAsync();
    }
}