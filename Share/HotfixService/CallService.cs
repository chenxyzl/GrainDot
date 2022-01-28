using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.ET;
using Base.Helper;
using Base.Serialize;
using Common;
using Message;

namespace Share.Component;

static public class CallComponentService
{
    static public SenderMessage GetCallback(this CallComponent self, ulong requestId)
    {
        return self.RequestCallbackDic[requestId];
    }
    
    static private ulong addRequestCallBack(this CallComponent self, SenderMessage senderMessage)
    {
        var rid = self.NextRequestId();
        self.RequestCallbackDic[rid] = senderMessage;
        return rid;
    }

    static public async ETTask<IResponse> Call(this CallComponent self, IActorRef other, IRequest request)
    {
        //request转id
        var tcs = ETTask<IResponse>.Create(true);
        var opcode = RpcManager.Instance.GetRequestOpcode(request.GetType());
        var rid = self.NextRequestId();
        self.RequestCallbackDic[rid] = new SenderMessage(TimeHelper.Now(), tcs, opcode);
        //
        var innerRequest = new InnerRequest {Opcode = opcode, Content = request.ToBinary(), Sn = rid};
        other.Tell(innerRequest);
        //
        var beginTime = TimeHelper.Now();
        var response = await tcs;
        var cost = TimeHelper.Now() - beginTime;
        //
        if (cost >= 100) self.Node.Logger.Warning($"call cost time:{cost} too long");

        //
        return response;
    }

    static public void Send(this CallComponent self, IActorRef other, IRequest request)
    {
        var innerRequest = new InnerRequest {Opcode = 1, Content = request.ToBinary(), Sn = 0};
        other.Tell(innerRequest);
    }


    static public Task Tick(this CallComponent self, long dt)
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

        return Task.CompletedTask;
    }
}