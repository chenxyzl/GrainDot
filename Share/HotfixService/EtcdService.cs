using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Base;
using dotnet_etcd;
using Etcdserverpb;
using Google.Protobuf;
using Message;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class EtcdService
{
    public static async Task Load(this EtcdComponent self)
    {
        await self.ConnectEtcd();
    }

    public static Task PreStop(this EtcdComponent self)
    {
        self.CancelSub();
        return Task.CompletedTask;
    }

    //跟随进程的临时kv
    public static async Task PutTemp(this EtcdComponent self, string k, string v)
    {
        await self.etcdClient.PutAsync(new PutRequest
        {
            Key = ByteString.CopyFromUtf8(k), Value = ByteString.CopyFromUtf8(v),
            Lease = self.LeaseId
        });
    }

    //持久化的kv
    public static async Task PutPersistent(this EtcdComponent self, string k, string v)
    {
        await self.etcdClient.PutAsync(new PutRequest
            {Key = ByteString.CopyFromUtf8(k), Value = ByteString.CopyFromUtf8(v)});
    }

    public static async Task Delete(this EtcdComponent self, string k)
    {
        await self.etcdClient.DeleteAsync(k);
    }

    public static async Task DeleteWithPrefix(this EtcdComponent self, string k)
    {
        await self.etcdClient.DeleteRangeAsync(k);
    }

    public static async Task<string> Get(this EtcdComponent self, string k, bool noException = true)
    {
        var result = await self.etcdClient.GetAsync(k);
        if (result.Kvs.Count == 0)
        {
            if (noException)
                return "";
            throw new CodeException(Code.Error, $"get key:{k} value is empty");
        }

        return result.Kvs[0].Value.ToStringUtf8();
    }

    public static async Task<List<string>> GetWithPrefix(this EtcdComponent self, string k)
    {
        var r = new List<string>();
        var result = await self.etcdClient.GetRangeAsync(k);
        foreach (var a in result.Kvs) r.Add(a.Value.ToStringUtf8());

        return r;
    }

    //只能在load函数中调用
    public static async void Watch(this EtcdComponent self, string k, Action<WatchEvent> func)
    {
        try
        {
            if (self.WatchCancellation.TryGetValue(k, out _)) A.Abort(Code.Error, "暂未实现list来保存对同一个k的多次监听");

            await EtcdComponent.LockAdd.WaitAsync();
            var t = new CancellationTokenSource();
            EtcdComponent.LockAdd.Release();
            self.etcdClient.Watch(k, a =>
            {
                foreach (var v in a) GlobalThreadSynchronizationContext.Instance.Post(state => { func(v); }, v);
            }, null, null, t.Token);
            self.WatchCancellation.TryAdd(k, t);
            EtcdComponent.LockAdd.Wait();
            await self.PutTemp("/ ", k);
            Thread.Sleep(1000);
        }
        catch (Exception)
        {
            if (self.WatchCancellation.TryGetValue(k, out var v)) v.Cancel();

            throw;
        }
    }

    //只能在load函数中调用
    public static async Task WatchPrefix(this EtcdComponent self, string k, Action<WatchEvent> func)
    {
        try
        {
            if (self.WatchCancellation.TryGetValue(k, out _)) A.Abort(Code.Error, "暂未实现list来保存对同一个k的多次监听");

            await EtcdComponent.LockAdd.WaitAsync();
            var t = new CancellationTokenSource();
            _ = Task.Run(() =>
            {
                EtcdComponent.LockAdd.Release();
                self.etcdClient.WatchRange(k, a =>
                {
                    foreach (var v in a) GlobalThreadSynchronizationContext.Instance.Post(state => { func(v); }, v);

                    ;
                }, null, null, t.Token);
            });
            self.WatchCancellation.TryAdd(k, t);
            EtcdComponent.LockAdd.Wait();
            await self.PutTemp("/WatchPrefixWait", k);
            Thread.Sleep(1000);
        }
        catch (Exception)
        {
            if (self.WatchCancellation.TryGetValue(k, out var v)) v.Cancel();

            throw;
        }
    }

    private static void CancelSub(this EtcdComponent self)
    {
        self.CancellationKeepLive.Cancel();
        foreach (var t in self.WatchCancellation) t.Value.Cancel();

        self.WatchCancellation.Clear();
    }

    public static void WatchTest(this EtcdComponent self, WatchEvent e)
    {
        Console.WriteLine($"watch {e.Key}, {e.Value}, {e.Type}");
    }

    public static async Task Test(EtcdComponent self)
    {
        self.Watch("/a/b", self.WatchTest);
        await self.WatchPrefix("/a/", self.WatchTest);
        //Thread.Sleep(5000);
        await self.PutTemp("/a/b", "11");
        await self.PutTemp("/a/c", "22");
        await self.PutPersistent("/h/h", "33");
        var a = await self.Get("/a/b");
        var b = await self.GetWithPrefix("/a/");
        //Thread.Sleep(5000);
        await self.Delete("/a/b");
        var c = await self.Get("/a/b");
        var d = await self.Get("/a/c");
        //Thread.Sleep(5000);
        await self.DeleteWithPrefix("/a/");
        var e = await self.Get("/a/b");
        var f = await self.Get("/a/c");
        Console.WriteLine($"final {a},{b},{c},{d}, {f}");
    }
}