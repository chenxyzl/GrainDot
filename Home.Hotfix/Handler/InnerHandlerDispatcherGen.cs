using Base;
using Base.Serializer;
using Home.Model;
using Message;
using System.Threading.Tasks;

namespace Home.Hotfix.Handler
{
    public partial class InnerHandlerDispatcher
    {
        public async Task<IResponse> DispatcherWithResult(BaseActor actor, InnerRequest message)
        {
            PlayerActor player = actor as PlayerActor;
            switch (message.Opcode)
            {
                case 10000: return await LoginHandler.LoginKeyHandler(player, SerializerHelper.FromBinary<AHPlayerLoginKeyAsk>(message.Content));
            }
            A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
            return null;
        }

        public async Task DispatcherNoResult(BaseActor actor, InnerRequest message)
        {
            PlayerActor player = actor as PlayerActor;
            switch (message.Opcode)
            {

            }
            A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        }
    }
}
