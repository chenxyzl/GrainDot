using Base;
using Base.Helper;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Boot
{
    class Boot
    {
        static async Task Main(string[] args)
        {
            //读取要启动的类型
            if (args.Length != 1)
            {
                A.Abort(PB.Code.Error, "参数长度唯一，且表示角色类型");
                return;
            }
            //读取参数
            RoleDef role = EnumHelper.FromString<RoleDef>(args[0]);
            //准备
            Game.Ready(role);
            //开始游戏
            await Game.Start();
            //开启无限循环
            Game.Loop();
            //结束游戏
            await Game.Stop();
        }
    }
}
