using System.Threading.Tasks;
using Base;
using Base.Serializer;
using Home.Model;
using Message;

namespace Home.Hotfix.Handler
{
    public partial class GateHandlerDispatcher
    {
        public async Task<IResponse> DispatcherWithResult(PlayerActor player, Request message)
        {
            switch (message.Opcode)
            {
                case 200000: return await LoginHandler.Ping(player, SerializerHelper.FromBinary<C2SPing>(message.Content));
            }
            A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
            return null;
        }

        public async Task DispatcherNoResult(PlayerActor player, Request message)
        {
            switch (message.Opcode)
            {
                case 200001: await LoginHandler.NotifyTest(player, SerializerHelper.FromBinary<CNotifyTest>(message.Content)); break;
            }
            A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        }
    }
}
