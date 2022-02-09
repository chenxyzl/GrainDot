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
        {
            target.Tell(msg);
        }
        else
        {
            GameServer.Instance.PlayerShardProxy.Tell(msg);
        }

        //
        var beginTime = TimeHelper.Now();
        var response = await tcs;
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
        {
            target.Tell(msg);
        }
        else
        {
            GameServer.Instance.WorldShardProxy.Tell(msg);
        }

        //
        var beginTime = TimeHelper.Now();
        var response = await tcs;
        var cost = TimeHelper.Now() - beginTime;
        //
        if (cost >= 100) self.Node.Logger.Warning($"call cost time:{cost} too long");

        //
        return response;
    }

    public static async ETTask ResumeActorThread(this CallComponent self)
    {
        //request转id
        var tcs = ETTask.Create(true);
        var rid = self.NextId();
        self.SyncCallbackDic[rid] = new SyncActorMessage(TimeHelper.Now(), tcs, rid);

        //
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