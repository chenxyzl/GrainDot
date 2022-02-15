using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.ET;
using Base.Helper;
using Base.Serialize;
using Common;
using Message;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class CallComponentService
{
    public static async ETTask<IResponse> AskPlayer(this CallComponent self, IRequestPlayer request,
        IActorRef? target = null)
    {
        //request转id
        var tcs = ETTask<IResponse>.Create(true);
        var opcode = RpcManager.Instance.GetRequestOpcode(request.GetType());
        var rid = self.NextId();
        self.RequestCallbackDic[rid] = new SenderMessage(TimeHelper.Now(), tcs, opcode);
        //
        var msg = new RequestPlayer {Opcode = opcode, Content = request.ToBinary(), Sn = rid};
        if (target != null)
            target.Tell(msg);
        else
            GameServer.Instance.PlayerShardProxy.Tell(msg);

        //
        var beginTime = TimeHelper.Now();
        var response = A.NotNull(await tcs);
        var cost = TimeHelper.Now() - beginTime;
        //
        if (cost >= 100) self.Node.Logger.Warning($"call cost time:{cost} too long");

        //
        return response;
    }

    public static async ETTask<IResponse> AskWorld(this CallComponent self, IRequestWorld request,
        IActorRef? target = null)
    {
        //request转id
        var tcs = ETTask<IResponse>.Create(true);
        var opcode = RpcManager.Instance.GetRequestOpcode(request.GetType());
        var rid = self.NextId();
        self.RequestCallbackDic[rid] = new SenderMessage(TimeHelper.Now(), tcs, opcode);
        //
        var msg = new RequestWorld {Opcode = opcode, Content = request.ToBinary(), Sn = rid};
        if (target != null)
            target.Tell(msg);
        else
            GameServer.Instance.WorldShardProxy.Tell(msg);

        //
        var beginTime = TimeHelper.Now();
        var response = A.NotNull(await tcs);
        var cost = TimeHelper.Now() - beginTime;
        //
        if (cost >= 100) self.Node.Logger.Warning($"call cost time:{cost} too long");

        //
        return response;
    }

    public static async ETTask ResumeActorThread(this CallComponent self)
    {
        //同步的远离是通过发送actor消息，在onrecive里切换线程。此时才是同步的切actor是激活的
        //load过程中不需要，因为ActorTaskScheduler.RunTask保证了actor的激活，只要保证load里只有db加载逻辑就不会出问题
        if (!self.Node.LoadComplete) return;

        //request转id
        var tcs = ETTask.Create(true);
        var rid = self.NextId();
        self.SyncCallbackDic[rid] = new SyncActorMessage(TimeHelper.Now(), tcs, rid);
        // GameServer.Instance.PlayerShardProxy.Tell(new ResumeActor {Sn = rid, PlayerId = self.Node.uid});
        // var _actor = GameServer.Instance.GetChild(self.Node.uid.ToString());
        // _actor.Tell(new ResumeActor {Sn = rid, PlayerId = self.Node.uid});
        //
        self.Node.GetSelf().Tell(new ResumeActor {Sn = rid, PlayerId = self.Node.uid});
        var beginTime = TimeHelper.Now();
        await tcs;
        var cost = TimeHelper.Now() - beginTime;
        //
        if (cost >= 100) GlobalLog.Warning($"sync cost time:{cost} too long");
        //
    }

    public static Task Tick(this CallComponent self, long dt)
    {
        var now = TimeHelper.Now();
        while (true)
        {
            if (self.RequestCallbackDic.Count == 0) break;

            var first = self.RequestCallbackDic.First();
            if (now - first.Value.CreateTime < GlobalParam.RPC_TIMEOUT_TIME) break;

            self.RequestCallbackDic.Remove(first.Key);
            first.Value.Tcs.SetException(new CodeException(Code.Error,
                $"rpc opcode:{first.Value.Opcode} time out, please check"));
        }

        while (true)
        {
            if (self.SyncCallbackDic.Count == 0) break;

            var first = self.SyncCallbackDic.First();
            if (now - first.Value.CreateTime < GlobalParam.RPC_TIMEOUT_TIME) break;

            self.SyncCallbackDic.Remove(first.Key);
            first.Value.Tcs.SetException(new CodeException(Code.Error,
                "sync time out, please check"));
        }

        return Task.CompletedTask;
    }
}