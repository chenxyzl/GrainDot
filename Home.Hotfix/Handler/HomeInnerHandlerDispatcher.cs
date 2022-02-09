using Akka.Actor;
using Base;
using Base.Serialize;
using Message;

namespace Home.Hotfix.Handler;
//为了高性能，对此文件的所有函数做了delegate缓存

[InnerRpc]
public partial class HomeInnerHandlerDispatcher : IInnerHandlerDispatcher
{
    public async void Dispatcher(BaseActor actor, IRequest request)
    {
        var message = request as RequestPlayer;
        var sender = actor.GetSender();
        var rpcType = A.RequireNotNull(RpcManager.Instance.GetRpcType(message.Opcode), Code.Error,
            $"inner opcode:{message.Opcode} not exit", true);
        if (rpcType == OpType.CS)
            try
            {
                var ret = await DispatcherWithResult(actor, message);
                sender.Tell(new InnerResponse
                    {Sn = message.Sn, Code = Code.Ok, Opcode = message.Opcode, Content = ret.ToBinary()});
            }
            catch (CodeException e)
            {
                sender.Tell(new InnerResponse {Sn = message.Sn, Code = e.Code, Opcode = message.Opcode});
                actor.Logger.Warning(e.ToString());
            }
        else if (rpcType == OpType.C)
            await DispatcherNoResult(actor, message);
        else
            A.Abort(Code.Error, $"opcode:{message.Opcode} type error", true);
    }
}