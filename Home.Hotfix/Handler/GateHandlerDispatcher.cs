using System;
using System.Threading.Tasks;
using Base;
using Base.Serialize;
using Home.Model;
using Message;

namespace Home.Hotfix.Handler;

[GateRpc]
public partial class GateHandlerDispatcher : IGateHandlerDispatcher
{
    public async Task Dispatcher(BaseActor actor, Request message)
    {
        var sender = actor.GetSender();
        sender.Tell(message, actor.GetSelf());
        var player = A.NotNull(actor as PlayerActor, Code.Error, "actor not player");
        var rpcType = A.NotNull(RpcManager.Instance.GetRpcType(message.Opcode), Code.Error,
            $"gate opcode:{message.Opcode} not exit", true);
        if (rpcType == OpType.CS)
            try
            {
                C2SLogin? loginMsg = null;
                //登录消息特殊处理
                if (message.Opcode == 200003)
                {
                    loginMsg = SerializeHelper.FromBinary<C2SLogin>(message.Content);
                    await player.LoginPreDeal(loginMsg);
                }

                //处理消息
                var ret = await DispatcherWithResult(player, message);
                await player.Send(ret, message.Opcode, message.Sn);

                //登录消息特殊处理
                if (loginMsg != null) player.LoginAfterDeal(loginMsg);
            }
            catch (CodeException e)
            {
                if (e.Serious) //严重错误不处理 继续上抛
                    throw;
                await player.SendError(message.Opcode, message.Sn, e.Code);
                player.Logger.Warning(e.ToString());
            }
            catch (Exception e)
            {
                player.Logger.Error(e.ToString());
                await player.SendError(message.Opcode, message.Sn, Code.Error);
            }
        else if (rpcType == OpType.C)
            await DispatcherNoResult(player, message);
        else
            A.Abort(Code.Error, $"opcode:{message.Opcode} type error", true);
    }
}