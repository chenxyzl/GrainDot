using Akka.Actor;
using Base;
using Base.Serializer;
using Message;
using World.Model;

namespace Home.Hotfix.Handler
{
    //为了高性能，对此文件的所有函数做了delegate缓存
    [InnerRpc]
    public partial class WorldInnerHandlerDispatcher : IInnerHandlerDispatcher
    {
        public async void Dispatcher(WorldSession session, InnerRequest message)
        {
            var sender = session.World.GetSender();
            var rpcType = A.RequireNotNull(RpcManager.Instance.GetRpcType(message.Opcode), Code.Error, $"inner opcode:{message.Opcode} not exit", true);
            if (rpcType == OpType.CS)
            {
                try
                {
                    IResponse ret = await DispatcherWithResult(session, message);
                    sender.Tell(new InnerResponse { Sn = message.Sn, Code = Code.Ok, Opcode = message.Opcode, Content = ret.ToBinary() });
                }
                catch (CodeException e)
                {
                    sender.Tell(new InnerResponse { Sn = message.Sn, Code = e.Code, Opcode = message.Opcode });
                    session.World.Logger.Warning(e.ToString());
                }
            }
            else if (rpcType == OpType.C)
            {
                await DispatcherNoResult(session, message);
            }
            else
            {
                A.Abort(Code.Error, $"opcode:{message.Opcode} type error", true);
            }
        }
    }
}
