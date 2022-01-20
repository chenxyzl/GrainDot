using System;
using System.Threading.Tasks;
using Base;
using Message;

namespace Login.Hotfix.Handler
{
    [HttpHandler(router:"/api/login")]
    public class RoleList : IHttpHandler
    {
        public Task<R> Run<R, T>(T message)
            where R : IResponse
            where T : IRequest
        {
            throw new NotImplementedException();
        }
    }
}
