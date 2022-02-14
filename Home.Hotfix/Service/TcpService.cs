using System;
using System.Threading.Tasks;
using Base;
using Base.Network;
using Home.Model;
using Home.Model.Component;

namespace Home.Hotfix.Service;

public static class TcpService
{
    public static async Task Load(this TcpComponent self)
    {
        await self.StartTcpServer<TcpChannel<PlayerChannel>>(self.port);
    }

    public static Task Start(this TcpComponent self)
    {
        return Task.CompletedTask;
    }


    public static Task PreStop(this TcpComponent self)
    {
        self._server.Close();
        return Task.CompletedTask;
    }

    public static Task Stop(this TcpComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Tick(this TcpComponent self)
    {
        //tick里检查所有的链接是否有超时未登陆的 如果有则关闭链接
        return Task.CompletedTask;
    }

    private static async Task StartTcpServer<T>(this TcpComponent self, ushort port) where T : TcpSocketConnection
    {
        self._server = await SocketBuilderFactory.GetTcpSocketServerBuilder<T>(port)
            .SetLengthFieldEncoder(2)
            .SetLengthFieldDecoder(ushort.MaxValue, 0, 2, 0, 2)
            .OnException(ex => { GlobalLog.Warning($"{self.GetType().Name}:{port} 服务端异常:{ex.Message}"); })
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
            .OnServerStarted(server =>
            {
                GlobalLog.Warning($"{self.GetType().Name}:{port} 服务启动");
            }).BuildAsync();
    }
}