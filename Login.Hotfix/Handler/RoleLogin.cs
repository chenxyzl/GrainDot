using System;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.Helper;
using Base.Serialize;
using Login.Model.State;
using Message;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Login.Hotfix.Handler;

[HttpHandler("/api/login")]
public class RoleLogin : HttpHandler<C2ARoleLogin, A2CRoleLogin>
{
    protected override async Task<A2CRoleLogin> Run(C2ARoleLogin data)
    {
        //查询全部，大概率会限制创建个数
        var list = await GameServer.Instance.GetComponent<DBComponent>()
            .Query<RoleSimpleState>((x => x.Account == data.Token));
        ulong playerId = data.Uid;
        if (data.Uid > 0) //检查角色是否存在
        {
            A.NotNull(list.Find(x => x.Id == playerId), Code.PlayerNotFound, "player not found");
        }
        else //创建新账号
        {
            playerId = IdGenerater.GenerateId();
        }

        var ask = new RequestPlayer
        {
            Opcode = 10000,
            Sn = 0,
            PlayerId = playerId,
            Content = new AHPlayerLoginKeyAsk().ToBinary()
        };
        var ans = await GameServer.Instance.PlayerShardProxy.Ask<InnerResponse>(ask, TimeSpan.FromSeconds(3));
        if (ans.Code != Code.Ok) throw new CodeException(ans.Code, ans.Code.ToString());

        var ret = SerializeHelper.FromBinary<A2CRoleLogin>(ans.Content);
        return ret;
    }
}