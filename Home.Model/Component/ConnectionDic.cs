using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using Base.Network;

namespace Home.Model.Component
{
    public class ConnectionDic : IGlobalComponent
    {
        private Dictionary<string, IBaseSocketConnection> connects = new Dictionary<string, IBaseSocketConnection>();
        public ConnectionDic()
        {
        }

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
            return connects[connectId];
        }

        public void AddConnection(IBaseSocketConnection connection)
        {
            var connectId = connection.ConnectionId;
            if (connects[connectId] != null)
            {
                GlobalLog.Error($"connectId:{connectId} repeated!");
            }
            connects[connectId] = connection;
        }

        public void RemoveConnection(string connectId)
        {
            if (connects[connectId] != null)
            {
                GlobalLog.Error($"connectId:{connectId} not found!");
            }
            connects.Remove(connectId);
        }
    }
}
