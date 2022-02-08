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
    private readonly string addrs;

    //"http://127.0.0.1:2379,http://127.0.0.1:2479"
    public EtcdComponent(string addresses)
    {
        addrs = "http://10.7.69.254:12379";
    }

    public EtcdClient etcdClient { private set; get; }
    public long LeaseId { private set; get; }
    public CancellationTokenSource CancellationKeepLive { get; } = new();

    public Dictionary<string, CancellationTokenSource> WatchCancellation { get; } = new();

    public static SemaphoreSlim LockAdd { get; } = new(1, 1);

    public async Task ConnectEtcd()
    {
        etcdClient = new EtcdClient(addrs);
        try
        {
            var res = await etcdClient.LeaseGrantAsync(new LeaseGrantRequest {TTL = 30});
            LeaseId = res.ID;
        }
        catch (Exception e)
        {
            GlobalLog.Error(e);
            throw;
        }

        _ = etcdClient.LeaseKeepAlive(LeaseId, CancellationKeepLive.Token);
        GlobalLog.Debug("etcd init success");
    }
}