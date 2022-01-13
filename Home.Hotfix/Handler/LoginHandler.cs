using Base;
using Home.Model;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Hotfix.Handler
{
    class LoginHandler : IHandler
    {
        public Dictionary<uint, RnHandler<MSG>> GetRnHandler<MSG>() where MSG : IMessage
        {
            var dic = new Dictionary<uint, RnHandler<MSG>>();
            return dic;
        }

        public Dictionary<uint, RpcHandler<REQ, RSQ>> GetRpcHandler<REQ, RSQ>()
            where REQ : IRequest
            where RSQ : IResponse
        {
            var typs = typeof(LoginHandler);

            var dic = new Dictionary<uint, RpcHandler<REQ, RSQ>>();

            var login = typs.GetMethod("Login");
            var a = (RpcHandler<C2S_Login, S2C_Login>)login.CreateDelegate(typeof(RpcHandler<C2S_Login, S2C_Login>), this);
            dic[100] = a;
            return dic;
        }

        public Task<S2C_Login> Login(C2S_Login req)
        {
            return Task.FromResult(new S2C_Login());
        }
    }
}
