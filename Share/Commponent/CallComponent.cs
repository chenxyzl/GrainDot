using System.Collections.Generic;
using Base;
using Base.Serialize;
using Message;

namespace Share.Model.Component;

public class CallComponent : IActorComponent
{
    private ulong _incId;
    private object lockInc = new();

    public CallComponent(BaseActor node) : base(node)
    {
    }

    public SortedDictionary<ulong, SenderMessage> RequestCallbackDic { get; } = new();
    public SortedDictionary<ulong, SyncActorMessage> SyncCallbackDic { get; } = new();

    public ulong NextId()
    {
        lock (lockInc)
        {
            return ++_incId;
        }
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

    public void ResumeActor(ResumeActor msg)
    {
        if (!SyncCallbackDic.TryGetValue(msg.Sn, out var senderMessage))
        {
            Node.Logger.Warning(
                $"resume message callback:{msg.Sn} not found; maybe time out");
            return;
        }

        SyncCallbackDic.Remove(msg.Sn);
        senderMessage.Tcs.SetResult();
    }
}