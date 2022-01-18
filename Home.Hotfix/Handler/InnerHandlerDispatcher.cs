using Base;
using Home.Model;
using Message;
using Base.Serializer;

namespace Home.Hotfix.Handler
{
    //为了高性能，对此文件的所有函数做了delegate缓存
    public partial class InnerHandlerDispatcher : IInnerHandlerDispatcher
    {
        public async void Dispatcher(BaseActor actor, InnerRequest message)
        {
            var player = actor as PlayerActor;
            var rpcitem = A.RequireNotNull(RpcManager.Instance.InnerRpcDic[message.Opcode], Code.Error, $"inner opcode:{message.Opcode} not exit", true);
            if (rpcitem.RpcType == RpcType.CS)
            {
                IResponse ret = await DispatcherWithResult(player, message);
                InnerResponse rsp = new InnerResponse { Sn = message.Sn, Code = Code.Ok, Opcode = message.Opcode, Content = ret.ToBinary() };
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
