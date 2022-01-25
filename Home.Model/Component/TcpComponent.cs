using System;
using System.Threading.Tasks;
using Base;
using Base.Network;

namespace Home.Model.Component;

public class TcpComponent : IGlobalComponent
{
    private ITcpSocketServer _server;

    public Task Load()
    {
        return Task.CompletedTask;
    }

    public async Task Start()
    {
        await StartTcpServer<PlayerChannel>(15000);
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

    private async Task StartTcpServer<T>(ushort port) where T : TcpSocketConnection
    {
        _server = await SocketBuilderFactory.GetTcpSocketServerBuilder<T>(port)
            .SetLengthFieldEncoder(2)
            .SetLengthFieldDecoder(ushort.MaxValue, 0, 2, 0, 2)
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