using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using Base.Network;

namespace Home.Model.Component;

public class ConnectionDicCommponent : IGlobalComponent
{
    private readonly Dictionary<string, IBaseSocketConnection> connects = new();
    private readonly object lockObj = new();

    public Task Load()
    {
        return Task.CompletedTask;
    }

    public Task PreStop()
    {
        return Task.CompletedTask;
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        return Task.CompletedTask;
    }

    public Task Tick()
    {
        return Task.CompletedTask;
    }
#nullable enable
    public IBaseSocketConnection? GetConnection(string connectId)
    {
        lock (lockObj)
        {
            return connects[connectId];
        }
    }

    public void AddConnection(IBaseSocketConnection connection)
    {
        lock (lockObj)
        {
            var connectId = connection.ConnectionId;
            if (connects[connectId] != null)
            {
                GlobalLog.Error($"connectId:{connectId} repeated, close old!");
                connects[connectId].Close();
            }

            connects[connectId] = connection;
        }
    }

    public void RemoveConnection(string connectId)
    {
        lock (lockObj)
        {
            if (connects[connectId] == null)
            {
                //actor 消毁时候是会再次断开链接
                //GlobalLog.Error($"connectId:{connectId} not found!");
            }
            else
            {
                connects.Remove(connectId);
            }
        }
    }
}