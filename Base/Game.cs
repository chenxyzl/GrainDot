using Base.Alg;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Base
{
    public static class Game
    {
        //当前的角色服务器
        static public GameServer gameServer;
        //退出标记
        static bool quitFlag = false;
        //退出标记监听
        static void WatchQuit() { Console.CancelKeyPress += (sender, e) => { quitFlag = true; }; }

        //加载程序集合
        static public void Reload()
        {
            AttrManager.Instance.Reload();
        }
        //准备
        static public void Ready()
        {
            GlobalLog.Warning("---dll加载中---");
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
            GlobalLog.Warning("---dll加载完成---");
        }
        //开始游戏
        static public Task Start()
        {
            GlobalLog.Warning("---启动中,请勿退出---");
            //开始
            //开始完成
            WatchQuit();
            GlobalLog.Warning("---启动完成,请勿退出---");
            return Task.CompletedTask;
        }
        static public void TickLoop()
        {
            while (!quitFlag)
            {
                Thread.Sleep(0);
            }
        }
        //结束游戏
        static public Task Stop()
        {
            GlobalLog.Warning("---停止中,请勿退出---");
            //结束
            //结束完成
            GlobalLog.Warning("---停止完成,请勿退出---");
            return Task.CompletedTask;
        }
    }
}
