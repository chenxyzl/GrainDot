using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Base;
using Base.Helper;
using Common;

namespace Robot;

public static class ClientManager
{
    private static readonly ConcurrentDictionary<ulong, Client> clients = new();

    private static async Task Main()
    {
        GlobalLog.Init(RoleType.NULL, CommandHelper.Instance.NodeId);
        IdGenerater.GlobalInit(CommandHelper.Instance.NodeId);
        RpcManager.Instance.ParseRpcItems();
        await Run();
    }

    private static async Task Run()
    {
        _ = Task.Run(() =>
        {
            while (true) Tick();
        });

        for (var i = 0; i < 10000; i++)
        {
            var client = new Client();
            var id = await client.Login();
            if (id == 0) continue;

            clients.TryAdd(id, client);
            Thread.Sleep(20);
        }


        Console.ReadLine();
    }

    public static void Tick()
    {
        var now = TimeHelper.Now();
        foreach (var client in clients)
        {
            // client.Value.Iick(now);
        }
    }
}