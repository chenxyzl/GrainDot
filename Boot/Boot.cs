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
        static void LoadDll()
        {

        }

        static Task Quit()
        {

        }

        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += Quit
            //读取要启动的类型
            if (args.Length != 1)
            {
                GlobalLog.Error("参数长度唯一，且表示角色类型");
                return;
            }
            //读取参数
            RoleDef role = EnumHelper.FromString<RoleDef>(args[0]);
            //准备
            Game.Ready();
            //检查状态
            if (Game.gameServer == null)
            {
                GlobalLog.Error($"服务器:{role}的实现类型未找到");
                return;
            }
            await Game.Start();
            //开启tick的无限循环
            Game.TickLoop();
            //结束游戏
            await Game.Stop();
        }
    }
}
