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
    partial class LoginHandler
    {
        public void IGateHandle(BaseActor actor, Response message)
        {
            var (sn, commond, msg) = (message.Sn, message.Opcode, message.Content);
        }
    }
}
