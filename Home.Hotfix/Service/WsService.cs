using System.Net;
using System.Threading.Tasks;
using Base;
using Base.Network;
using Home.Model;
using Home.Model.Component;

namespace Home.Hotfix.Service;

[Service(typeof(WsComponent))]
public static class WsService
{
    public static async Task Load(this WsComponent self)
    {
        await self.StartWsServer<WebSocketChannel<PlayerChannel>>(self.ip, self.port);
    }


    public static Task Start(this WsComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task PreStop(this WsComponent self)
    {
        self._server.Close();
        return Task.CompletedTask;
    }

    public static Task Stop(this WsComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Tick(this WsComponent self, long now)
    {
        //tick里检查所有的链接是否有超时未登陆的 如果有则关闭链接
        return Task.CompletedTask;
    }

    public static async Task StartWsServer<T>(this WsComponent self, IPAddress ip, ushort port)
        where T : WebSocketConnection
    {
        self._server = await SocketBuilderFactory.GetWebSocketServerBuilder<T>(ip, port)
            // .OnException(ex => { GlobalLog.Warning($"{self.GetType().Name} {ip}:{port} 服务端异常:{ex.Message}"); })
            // .OnNewConnection((server, connection) =>
            // {
            //     GameServer.Instance.GetComponent<ConnectionDicCommponent>().AddConnection(connection);
            // })
            // .OnConnectionClose(
            //     (server, connection) =>
            //     {
            //         GameServer.Instance.GetComponent<ConnectionDicCommponent>()
            //             .RemoveConnection(connection.ConnectionId);
            //     })
            .OnServerStarted(server => { GlobalLog.Warning($"{self.GetType().Name} {ip}:{port} 服务启动"); }).BuildAsync();
    }
}