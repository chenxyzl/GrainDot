using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Helper;
using Base.Network;

namespace Home.Model.Component;

public class ConnectionDicCommponent : IGlobalComponent
{
    private readonly SortedDictionary<ulong, string> _waitAuthed = new();
    private readonly Dictionary<string, ICustomChannel> connects = new();
    private readonly object lockObj = new();

    public ICustomChannel? GetConnection(string? connectId)
    {
        lock (lockObj)
        {
            if (connectId == null) return null;
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
            _waitAuthed.TryAdd(IdGenerater.GenerateId(), connectId);
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

            //因为正在登录中人数一定不多。所以这里lock写在while里。
            lock (lockObj)
            {
                var first = _waitAuthed.First();
                if (IdGenerater.ParseTime(first.Key) + 60_000 > now) break;
                //开始处理超时
                _waitAuthed.Remove(first.Key);
                var connection = GetConnection(first.Value);
                if (connection != null && !connection.authed)
                    //close 会触发删除，所以这里不用管
                    connection.Close();
            }
        }
    }
}