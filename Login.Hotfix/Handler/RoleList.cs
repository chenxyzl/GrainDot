using System.Threading.Tasks;
using Base;
using Message;

namespace Login.Hotfix.Handler;

[HttpHandler("/api/rolelist")]
public class RoleList : HttpHandler<C2ALogin, A2CLogin>
{
    protected override Task<A2CLogin> Run(C2ALogin data)
    {
        //todo 本地查询数据库 返回客户端账号列表
        return Task.FromResult(new A2CLogin());
    }
}