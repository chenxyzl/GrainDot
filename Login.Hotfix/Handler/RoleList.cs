using System;
using System.Threading.Tasks;
using Base;
using Message;

namespace Login.Hotfix.Handler
{
    [HttpHandler(router: "/api/login")]
    public class RoleList : HttpHandler<C2ALogin, A2CLogin>
    {
        protected override Task<C2ALogin> Run(A2CLogin data)
        {
            //todo 登录http实现
            throw new NotImplementedException();
        }
    }
}
