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

    //new string[] { "http://127.0.0.1:2379" }
    public EtcdComponent(string[] addresses)
    {
        addrs = string.Join(",", addresses);
    }

    public EtcdClient etcdClient { private set; get; }
    public long LeaseId { private set; get; }
    public CancellationTokenSource CancellationKeepLive { get; } = new();

    public Dictionary<string, CancellationTokenSource> WatchCancellation { get; } = new();

    public static SemaphoreSlim LockAdd { get; } = new(1, 1);

    public async Task ConnectEtcd()
    {
        etcdClient = new EtcdClient(addrs);
        var res = await etcdClient.LeaseGrantAsync(new LeaseGrantRequest {TTL = 30});
        LeaseId = res.ID;
        _ = Task.Run(() => etcdClient.LeaseKeepAlive(LeaseId, CancellationKeepLive.Token));
    }
}