using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Base;
using Base.Serialize;
using Common;
using Message;
using SharpCompress.Archives;

namespace Login.Hotfix.Handler;

[HttpHandler("/api/login")]
public class Login : HttpHandler<C2ARoleLogin, A2CRoleLogin>
{
    protected override async Task<A2CRoleLogin> Run(C2ARoleLogin data)
    {
        var ask = new RequestPlayer
        {
            Opcode = 10000,
            Sn = 0,
            PlayerId = data.Uid,
            Content = new AHPlayerLoginKeyAsk().ToBinary()
        };
        var ans = await GameServer.Instance.PlayerShardProxy.Ask<InnerResponse>(ask);
        if (ans.Code != Code.Ok)
        {
            throw new CodeException(ans.Code, ans.Code.ToString());
        }

        var ret = SerializeHelper.FromBinary<A2CRoleLogin>(ans.Content);
        return ret;
    }
}