using System.Collections.Generic;
using Base;
using Base.Serialize;
using Message;

namespace Share.Model.Component;

public class CallComponent : IActorComponent<BaseActor>
{
    private readonly object lockInc = new();
    private ulong _incId;

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

    public void RunResponse(InnerResponse response)
    {
        if (!RequestCallbackDic.TryGetValue(response.Sn, out var senderMessage))
        {
            Node.Logger.Warning(
                $"inner message callback:{response.Sn} Name:{response.Opcode} Code:{response.Code} not found; maybe time out");
            return;
        }

        RequestCallbackDic.Remove(response.Sn);
        if (response.Code != Code.Ok)
        {
            senderMessage.Tcs.SetException(new CodeException(response.Code, response.Code.ToString()));
        }
        else
        {
            var retType = RpcManager.Instance.GetResponseOpcode(response.Opcode);
            var ret = A.NotNull(SerializeHelper.FromBinary(retType, response.Content) as IResponse);
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