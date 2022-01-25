using Akka.Actor;
using Base;
using Base.ET;
using Base.Helper;
using Base.Serialize;
using Common;
using Message;

namespace Share.Model.Component;

public class CallComponent : IActorComponent
{
    private ulong _requestIncId;

    public CallComponent(BaseActor node) : base(node)
    {
    }

    public SortedDictionary<ulong, SenderMessage> RequestCallbackDic { get; } = new();

    public ulong NextRequestId()
    {
        return ++_requestIncId;
    }

    public SenderMessage GetCallback(ulong requestId)
    {
        return RequestCallbackDic[requestId];
    }

    public void RemoveRequestCallBack(ulong requestId)
    {
        RequestCallbackDic.Remove(requestId);
    }

    public ulong AddRequestCallBack(SenderMessage senderMessage)
    {
        var rid = NextRequestId();
        RequestCallbackDic[rid] = senderMessage;
        return rid;
    }

    public async ETTask<IResponse> Call(IActorRef other, IRequest request)
    {
        //request转id
        var tcs = ETTask<IResponse>.Create(true);
        var opcode = RpcManager.Instance.GetRequestOpcode(request.GetType());
        var callComponent = Node.GetComponent<CallComponent>();
        var rid = callComponent.AddRequestCallBack(new SenderMessage(TimeHelper.Now(), tcs, opcode));
        //
        var innerRequest = new InnerRequest {Opcode = opcode, Content = request.ToBinary(), Sn = rid};
        other.Tell(innerRequest);
        //
        var beginTime = TimeHelper.Now();
        var response = await tcs;
        var cost = TimeHelper.Now() - beginTime;
        //
        if (cost >= 100) Node.Logger.Warning($"call cost time:{cost} too long");

        //
        return response;
    }

    public void Send(IActorRef other, IRequest request)
    {
        var innerRequest = new InnerRequest {Opcode = 1, Content = request.ToBinary(), Sn = 0};
        other.Tell(innerRequest);
    }

    public void RunResponse(InnerResponse respone)
    {
        if (!RequestCallbackDic.TryGetValue(respone.Sn, out var senderMessage))
        {
            Node.Logger.Warning(
                $"inner message callback:{respone.Sn} Name:{respone.Opcode} Code:{respone.Code} not found; maybe time out");
            return;
        }

        RequestCallbackDic.Remove(respone.Sn);
        if (respone.Code != Code.Ok)
        {
            senderMessage.Tcs.SetException(new CodeException(respone.Code, respone.Code.ToString()));
        }
        else
        {
            var retType = RpcManager.Instance.GetResponseOpcode(respone.Opcode);
            var ret = SerializeHelper.FromBinary(retType, respone.Content) as IResponse;
            senderMessage.Tcs.SetResult(ret);
        }
    }


    public Task Tick(long dt)
    {
        var now = TimeHelper.Now();
        while (true)
        {
            if (RequestCallbackDic.Count == 0) break;

            var first = RequestCallbackDic.First();
            if (now - first.Value.CreateTime < GlobalParam.RPC_TIMEOUT_TIME) break;

            RequestCallbackDic.Remove(first.Key);
            first.Value.Tcs.SetException(new CodeException(Code.Error,
                $"rpc opcode:{first.Value.Opcode} time out, please check"));
        }

        return Task.CompletedTask;
    }
}