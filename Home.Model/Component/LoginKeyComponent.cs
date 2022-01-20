using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.Helper;
using Base.Network;

namespace Home.Model.Component
{
    public class LoginKeyComponent : IGlobalComponent
    {
        private Dictionary<string, IActorRef> loginKeys = new Dictionary<string, IActorRef>();

        private Dictionary<IActorRef, string> loginRefs = new Dictionary<IActorRef, string>();
        private Random random = new Random();
        private object lockObj = new object();
        public LoginKeyComponent()
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
        public IActorRef? KeyToPlayerRefAndRemove(string key)
        {
            lock (lockObj)
            {
                var playerRef = loginKeys[key];
                loginKeys.Remove(key);
                loginRefs.Remove(playerRef);
                return playerRef;
            }
        }

        public string AddPlayerRef(IActorRef playerRef)
        {
            lock (lockObj)
            {
                while (true)
                {
                    var key = random.RandUInt64().ToString();
                    if (loginKeys[key] != null)
                    {
                        continue;
                    }
                    //删除老的
                    var old = loginRefs[playerRef];
                    if (old != null)
                    {
                        loginKeys.Remove(old);
                        loginRefs.Remove(playerRef);
                    }
                    //更新
                    loginKeys[key] = playerRef;
                    loginRefs[playerRef] = key;
                    return key;
                }

            }
        }
        //不需要再要删除接口了。因为获取不到player就认为不可以登录
    }
}
