using System;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.Helper;
using Base.Serialize;
using Message;

namespace Login.Hotfix.Handler;

[HttpHandler("/api/p0")]
public class PerformanceTest : HttpHandler<C2APerformanceTest, A2CPerformanceTest>
{
    protected override async Task<A2CPerformanceTest> Run(C2APerformanceTest data)
    {
        var ask = new RequestPlayer
        {
            Opcode = 10000,
            Sn = 0,
            PlayerId = IdGenerater.GenerateId(),
            Content = new AHPlayerLoginKeyAsk().ToBinary()
        };
        var ans = await GameServer.Instance.PlayerShardProxy.Ask<InnerResponse>(ask, TimeSpan.FromSeconds(3));
        if (ans.Code != Code.Ok) throw new CodeException(ans.Code, ans.Code.ToString());
        var ret = SerializeHelper.FromBinary<A2CLogin>(ans.Content);

        return new A2CPerformanceTest();
    }
}