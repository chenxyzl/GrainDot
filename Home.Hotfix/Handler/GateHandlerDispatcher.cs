using Base;
using Base.Serializer;
using Home.Model;
using Message;

namespace Home.Hotfix.Handler
{
    public partial class GateHandlerDispatcher : IGateHandlerDispatcher
    {
        public async void Dispatcher(BaseActor actor, Request message)
        {
            var player = actor as PlayerActor;
            var rpcitem = A.RequireNotNull(RpcManager.Instance.GateRpcDic[message.Opcode], Code.Error, $"gate opcode:{message.Opcode} not exit", true);
            if (rpcitem.RpcType == RpcType.CS)
            {
                IResponse ret = await DispatcherWithResult(player, message);
                Response rsp = new Response { Sn = message.Sn, Code = Code.Ok, Opcode = message.Opcode, Content = ret.ToBinary() };
                await player.Send(rsp);
            }
            else if (rpcitem.RpcType == RpcType.C)
            {
                await DispatcherNoResult(player, message);
            }
            else
            {
                A.Abort(Code.Error, $"opcode:{rpcitem.Opcode} type error", true);
            }
        }
    }
}
