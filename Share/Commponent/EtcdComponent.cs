using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Base;
using dotnet_etcd;
using Etcdserverpb;

namespace Share.Model.Component;

public class EtcdComponent : IGlobalComponent
{
    public readonly string Addrs;

    //"http://127.0.0.1:2379,http://127.0.0.1:2479"
    public EtcdComponent(string addresses)
    {
        Addrs = addresses;
    }

    public EtcdClient EtcdClient { set; get; } = null!;
    public long LeaseId { set; get; }
    public CancellationTokenSource CancellationKeepLive { get; } = new();

    public Dictionary<string, CancellationTokenSource> WatchCancellation { get; } = new();

    public static SemaphoreSlim LockAdd { get; } = new(1, 1);
}