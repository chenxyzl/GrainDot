using System;
using Base;
using Base.Serialize;
using Home.Model;
using Message;

namespace Home.Hotfix.Handler;

[GateRpc]
public partial class GateHandlerDispatcher : IGateHandlerDispatcher
{
    public async void Dispatcher(BaseActor actor, Request message)
    {
        var player = actor as PlayerActor;
        var rpcType = A.RequireNotNull(RpcManager.Instance.GetRpcType(message.Opcode), Code.Error,
            $"gate opcode:{message.Opcode} not exit", true);
        if (rpcType == OpType.CS)
            try
            {
                var ret = await DispatcherWithResult(player, message);
                await player.Send(new Response
                    {Sn = message.Sn, Code = Code.Ok, Opcode = message.Opcode, Content = ret.ToBinary()});
            }
            catch (CodeException e)
            {
                if (e.Serious) //严重错误不处理 继续上抛
                {
                    throw e;
                }

                await player.Send(new Response {Sn = message.Sn, Code = e.Code, Opcode = message.Opcode});
                player.Logger.Warning(e.ToString());
            }
            catch (Exception e)
            {
                player.Logger.Error(e.ToString());
                await player.Send(new Response {Sn = message.Sn, Code = Code.Error, Opcode = message.Opcode});
            }
        else if (rpcType == OpType.C)
            await DispatcherNoResult(player, message);
        else
            A.Abort(Code.Error, $"opcode:{message.Opcode} type error", true);
    }
}