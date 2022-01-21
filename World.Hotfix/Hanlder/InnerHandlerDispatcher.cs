using Base;
using Home.Model;
using Message;

namespace Home.Hotfix.Handler
{
    //为了高性能，对此文件的所有函数做了delegate缓存
    public partial class InnerHandlerDispatcher : IInnerHandlerDispatcher
    {
        public async void Dispatcher(BaseActor actor, InnerRequest message)
        {
            var player = actor as PlayerActor;
            var rpcitem = A.RequireNotNull(RpcManager.Instance.InnerRpcDic[message.Opcode], Code.Error, $"inner opcode:{message.Opcode} not exit", true);
            if (rpcitem.RpcType == OpType.CS)
            {
                IResponse ret = await DispatcherWithResult(player, message);
                Response rsp = new Response { Sn = message.Sn, Code = Code.Ok, Opcode = message.Opcode, Content = ret.ToBinary() };
                await player.Send(rsp);
            }
            else if (rpcitem.RpcType == OpType.C)
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
