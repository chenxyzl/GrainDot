using Home.Model.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Hotfix.Service
{
    public static class PlayerBagService
    {
        public static Task Load(this BagComponent self)
        {
            return Task.CompletedTask;
        }
    }
}
