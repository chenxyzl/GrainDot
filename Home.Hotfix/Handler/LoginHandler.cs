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
    static class LoginHandler
    {
        [HandlerMethod(1000)]
        static Task<S2C_Login> Login(this PlayerActor player, C2S_Login req)
        {
            return Task.FromResult(new S2C_Login());
        }
    }
}
