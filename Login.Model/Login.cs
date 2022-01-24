using Base;
using Base.Network.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Model
{
    public class Login : GameServer
    {
        public Login() : base(Common.RoleType.Login) { }

        public override void RegisterGlobalComponent()
        {
            AddComponent<HttpComponent>(20001);
        }
    }
}
