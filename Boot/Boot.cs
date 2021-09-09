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

        static void Main(string[] args)
        {
            //读取要启动的类型
            if (args.Length != 1)
            {
                GlobalLog.Error("参数长度唯一，且表示角色类型");
                return;
            }
            RoleDef role = EnumHelper.FromString<RoleDef>(args[0]);
            //程序集合初始化
            Game.Reload();
            //获取服务器类型
            foreach (var x in Game.GetServers())
            {
                if (x.Name == RoleDef.All.ToString())
                {
                    Game.gameServer = Activator.CreateInstance(x) as GameServer;
                    break;
                }
            }
            if (Game.gameServer == null)
            {
                GlobalLog.Error($"服务器:{role}的实现类型未找到");
                return;
            }
            while (true)
            {
                Thread.Sleep(0);
            }
        }

        Task FastBoot()
        {
            return Task.CompletedTask;
        }

        Task NormalBoot()
        {
            return Task.CompletedTask;
        }
    }
}
