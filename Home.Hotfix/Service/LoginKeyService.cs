using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Base.Helper;
using Home.Model.Component;

namespace Home.Hotfix.Service;

public static class LoginKeyService
{
    public static Task Load(this LoginKeyComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Tick(this LoginKeyComponent self)
    {
        while (true)
        {
            if (self.timeKeys.Count == 0) break;

            var item = self.timeKeys.First();
            var now = TimeHelper.Now();
            if (item.Key - now < 15000) break;

            //因为正在登录中人数一定不多。所以这里lock写在while里。
            lock (self.lockObj)
            {
                //让对应的loginKey失效
                self.timeKeys.Remove(item.Key);
                var playerRef = self.loginKeys[item.Value];
                self.loginKeys.Remove(item.Value);
                self.loginRefs.Remove(playerRef);
            }
        }

        //检查长时间未连接的
        return Task.CompletedTask;
    }
#nullable enable
    public static IActorRef? KeyToPlayerRefAndRemove(this LoginKeyComponent self, string key)
    {
        lock (self.lockObj)
        {
            if (self.loginKeys.TryGetValue(key, out var playerRef))
            {
                self.loginKeys.Remove(key);
                self.loginRefs.Remove(playerRef);
            }

            return playerRef;
        }
    }

    public static string AddPlayerRef(this LoginKeyComponent self, IActorRef playerRef)
    {
        lock (self.lockObj)
        {
            while (true)
            {
                var key = self.random.RandUInt64().ToString();
                if (self.loginKeys.ContainsKey(key)) continue;

                //删除老的
                self.loginRefs.TryGetValue(playerRef, out var old);
                if (old != null)
                {
                    self.loginKeys.Remove(old);
                    self.loginRefs.Remove(playerRef);
                }

                //更新
                self.loginKeys.TryAdd(key, playerRef);
                self.loginRefs.TryAdd(playerRef, key);
                self.timeKeys.TryAdd(TimeHelper.NowSeconds(), key);
                return key;
            }
        }
    }
    //不需要再要删除接口了。因为获取不到player就认为不可以登录
}