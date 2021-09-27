using Base.Alg;
using Base.Helper;
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
        static public ServerState ServerState = ServerState.Unknow;
        //当前的角色服务器
        static public GameServer GameServer;
        //退出标记
        static bool _quitFlag = false;
        //退出标记监听
        static void WatchQuit()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                if (_quitFlag)
                {
                    return;
                }
                _quitFlag = true;
                e.Cancel = true;
            };
        }

        //加载程序集合
        static public void Reload()
        {
            AttrManager.Instance.Reload();
        }
        //准备
        static public void Ready(RoleDef role, Type gsType)
        {
            GlobalLog.Warning($"---{role}加载中---");
            //程序集合初始化
            Reload();
            //创建服务器启动类
            GameServer = Activator.CreateInstance(gsType) as GameServer;
            GlobalLog.Warning($"---{role}加载完成---");
        }
        //开始游戏
        static public async Task Start()
        {
            GlobalLog.Warning($"---{GameServer.role}启动中,请勿强关---");
            //开始
            await GameServer.StartSystem();
            //开始完成
            GlobalLog.Warning($"---{GameServer.role}启动完成---");
            WatchQuit();
        }
        static public void Loop()
        {
            GlobalLog.Warning($"---{GameServer.role}开启loop---");
            while (!_quitFlag)
            {
                Thread.Sleep(1);
            }
            GlobalLog.Warning($"---{GameServer.role}退出loop---");
        }
        //结束游戏
        static public async Task Stop()
        {
            GlobalLog.Warning($"---{GameServer.role}停止中,请勿强关---");
            await GameServer.StopSystem();
            GlobalLog.Warning($"---{GameServer.role}停止完成---");
        }

        static public async Task Boot(RoleDef role, Type gsType)
        {
            //准备
            Ready(role, gsType);
            //开始游戏
            await Start();
            //开启无限循环
            Loop();
            //结束游戏
            await Stop();
        }
    }
}
