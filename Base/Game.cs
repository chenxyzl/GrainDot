using Base.Alg;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public static class Game
    {
        //当前的角色服务器
        static public GameServer gameServer;
        //加载程序集合
        static public void Reload()
        {
            AttrManager.Instance.Reload();
        }
        //准备
        static public void Ready()
        {
            //程序集合初始化
            Reload();
            //获取服务器类型
            foreach (var x in AttrManager.Instance.GetServers())
            {
                if (x.Name == RoleDef.All.ToString())
                {
                    gameServer = Activator.CreateInstance(x) as GameServer;
                    break;
                }
            }
        }
        //开始游戏
        static public void Start()
        {

        }
        //全局tick
        static public void Tick()
        {

        }
        //结束游戏
        static public void Stop()
        {

        }
    }
}
