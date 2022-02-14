using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Helper;
using Base.Network;

namespace Home.Model.Component;

public class ConnectionDicCommponent : IGlobalComponent
{
    private readonly SortedDictionary<long, string> _waitAuthed = new();
    private readonly Dictionary<string, ICustomChannel> connects = new();
    private readonly object lockObj = new();

    public ICustomChannel? GetConnection(string connectId)
    {
        lock (lockObj)
        {
            connects.TryGetValue(connectId, out var v);
            return v;
        }
    }

    public void AddConnection(ICustomChannel connection)
    {
        lock (lockObj)
        {
            var connectId = connection.ConnectionId;
            if (connects.TryGetValue(connectId, out var conn))
            {
                GlobalLog.Error($"connectId:{connectId} repeated, close old!");
                conn.Close();
                connects.Remove(connectId);
            }

            connects.TryAdd(connectId, connection);
        }
    }

    public bool RemoveConnection(string connectId)
    {
        lock (lockObj)
        {
            if (connects.TryGetValue(connectId, out var conn))
            {
                //actor 消毁时候是会再次断开链接
                //GlobalLog.Error($"connectId:{connectId} not found!");
                connects.Remove(connectId);
                return true;
            }

            return false;
        }
    }

    //长时间未验证成功的链接需要断开
    public void Tick()
    {
        var now = TimeHelper.Now();
        while (true)
        {
            if (_waitAuthed.Count == 0) break;

            var first = _waitAuthed.First();
            if (first.Key + 60_000 > now) break;

            _waitAuthed.Remove(first.Key);
            var connection = GetConnection(first.Value);
            if (connection != null && !connection.authed)
                //close 会触发删除，所以这里不用管
                connection.Close();
        }
    }
}