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
    public EtcdClient etcdClient { private set; get; }
    public long LeaseId { private set; get; }
    public CancellationTokenSource CancellationKeepLive { private set; get; } = new CancellationTokenSource();

    public Dictionary<string, CancellationTokenSource> WatchCancellation { private set; get; } =
        new Dictionary<string, CancellationTokenSource>();

    public static SemaphoreSlim LockAdd { private set; get; } = new SemaphoreSlim(1, 1);

    //new string[] { "http://127.0.0.1:2379" }
    public EtcdComponent(string[] addresses)
    {
        addrs = string.Join(",", addresses);
    }

    public async Task ConnectEtcd()
    {
        etcdClient = new EtcdClient(addrs);
        LeaseGrantResponse res = await etcdClient.LeaseGrantAsync(new LeaseGrantRequest {TTL = 30});
        LeaseId = res.ID;
        _ = Task.Run(() => etcdClient.LeaseKeepAlive(LeaseId, CancellationKeepLive.Token));
    }
}