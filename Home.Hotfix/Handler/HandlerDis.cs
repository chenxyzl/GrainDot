using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Home.Model;

namespace Home.Hotfix.Handler
{

    //为了高性能，对此文件的所有函数做了delegate缓存
    public class HandlerDis
    {
        Task<object> innerRpcCall(PlayerActor actor, object message)
        {
            //消息序列化为二进制
            //

            var login = new LoginHandler();
            switch (a)
            {
                case 0:
                    {
                        //序列化
                        login.Login(actor,)
                        break;
                    }
            }
            return Task.FromResult(new object());
        }

        //为了高性能
        Task innerRnCall()
        {
            return Task.CompletedTask;

        }
    }
}
