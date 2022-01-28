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

namespace Share.Model.Component;

public class CallComponent : IActorComponent
{
    private ulong _incId;

    public SortedDictionary<ulong, SenderMessage> RequestCallbackDic { get; } = new();
    public SortedDictionary<ulong, SyncActorMessage> SyncCallbackDic { get; } = new();

    public CallComponent(BaseActor node) : base(node)
    {
    }

    public ulong NextId()
    {
        return ++_incId;
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
}