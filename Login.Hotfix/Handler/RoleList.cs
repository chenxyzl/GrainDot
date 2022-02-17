using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Base;
using Login.Model.State;
using Message;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Login.Hotfix.Handler;

[HttpHandler("/api/rolelist")]
public class RoleList : HttpHandler<C2AGetRoleList, A2CGetRoleList>
{
    protected override async Task<A2CGetRoleList> Run(C2AGetRoleList data)
    {
        var list = await GameServer.Instance.GetComponent<DBComponent>()
            .Query<RoleSimpleState>(x => x.Account == data.Token, null);
        await GameServer.Instance.GetComponent<DBComponent>().CreateIndexAsync<RoleSimpleState>("Account");

        //转换
        var o = new A2CGetRoleList {Rols = new()};
        foreach (var roleSumpState in list)
        {
            o.Rols.Add(new SimpleRole
            {
                Uid = roleSumpState.Id,
                Tid = roleSumpState.Tid,
                Name = roleSumpState.Name,
                LastLoginTime = roleSumpState.LastLoginTime,
                LastOfflineTime = roleSumpState.LastOfflineTime,
                Exp = roleSumpState.Exp
            });
        }

        return o;
    }
}