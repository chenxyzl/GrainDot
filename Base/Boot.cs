using Akka.Actor;
using Akka.Cluster.Sharding;
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
    public static class Boot
    {
        static public ServerState ServerState = ServerState.Unknow;
        //当前的角色服务器
        static public GameServer GameServer;
        //退出标记
        static bool _quitFlag = false;
        static long lastTime = 0;
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
        static void Reload()
        {
            HotfixManager.Instance.Reload();
        }
        //准备
        static void Ready(Type gsType)
        {
            GlobalLog.Warning($"---{gsType.Name}加载中---");
            //程序集合初始化
            Reload();
            //创建服务器启动类
            GameServer = Activator.CreateInstance(gsType) as GameServer;
            GlobalLog.Warning($"---{gsType.Name}加载完成---");
        }
        //开始游戏
        static async Task Start(string typeName, Props p, HashCodeMessageExtractor extractor)
        {
            GlobalLog.Warning($"---{GameServer.role}启动中,请勿强关---");
            //开始
            await GameServer.StartSystem(typeName, p, extractor);
            lastTime = TimeHelper.Now();
            //开始完成
            GlobalLog.Warning($"---{GameServer.role}启动完成---");
            WatchQuit();
        }
        //开始游戏
        static async Task Start()
        {
            GlobalLog.Warning($"---{GameServer.role}启动中,请勿强关---");
            //开始
            await GameServer.StartSystem();
            lastTime = TimeHelper.Now();
            //开始完成
            GlobalLog.Warning($"---{GameServer.role}启动完成---");
            WatchQuit();
        }
        static void Loop()
        {
            GlobalLog.Warning($"---{GameServer.role}开启loop---");
            while (!_quitFlag)
            {
                Thread.Sleep(1);
                var now = TimeHelper.Now();
                //1000毫秒tick一次
                if (now - lastTime < 1000) continue;
                lastTime += 1000;
                _ = GameServer.Tick();
            }
            GlobalLog.Warning($"---{GameServer.role}退出loop---");
        }
        //结束游戏
        static async Task Stop()
        {
            GlobalLog.Warning($"---{GameServer.role}停止中,请勿强关---");
            await GameServer.StopSystem();
            GlobalLog.Warning($"---{GameServer.role}停止完成---");
        }

        static public async Task Run(Type gsType, string typeName, Props p, HashCodeMessageExtractor extractor)
        {
            //准备
            Ready(gsType);
            //开始游戏
            await Start(typeName, p, extractor);
            //开启无限循环
            Loop();
            //结束游戏
            await Stop();
        }
        static public async Task Run(Type gsType)
        {
            //准备
            Ready(gsType);
            //开始游戏
            await Start();
            //开启无限循环
            Loop();
            //结束游戏
            await Stop();
        }
    }
}
