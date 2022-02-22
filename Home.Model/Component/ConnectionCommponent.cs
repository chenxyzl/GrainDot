using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Helper;
using Base.Network;

namespace Home.Model.Component;

public class ConnectionCommponent : IGlobalComponent
{
    public readonly SortedDictionary<ulong, string> WaitAuthed = new();
    private readonly Dictionary<string, ICustomChannel> _connects = new();
    public readonly object LockObj = new();

    public ICustomChannel? GetConnection(string? connectId)
    {
        lock (LockObj)
        {
            if (connectId == null) return null;
            _connects.TryGetValue(connectId, out var v);
            return v;
        }
    }

    public void AddConnection(ICustomChannel connection)
    {
        lock (LockObj)
        {
            var connectId = connection.ConnectionId;
            if (_connects.TryGetValue(connectId, out var conn))
            {
                GlobalLog.Error($"connectId:{connectId} repeated, close old!");
                conn.Close();
                _connects.Remove(connectId);
            }

            _connects.TryAdd(connectId, connection);
            WaitAuthed.TryAdd(IdGenerater.GenerateId(), connectId);
        }
    }

    public bool RemoveConnection(string connectId)
    {
        lock (LockObj)
        {
            if (_connects.TryGetValue(connectId, out var conn))
            {
                //actor 消毁时候是会再次断开链接
                //GlobalLog.Error($"connectId:{connectId} not found!");
                _connects.Remove(connectId);
                return true;
            }

            return false;
        }
    }
}