using System.Threading.Tasks;
using Base;
using Base.Serialize;
using Home.Model;
using Message;

namespace Home.Hotfix.Handler;

public partial class HomeInnerHandlerDispatcher
{
    public async Task<IResponse> DispatcherWithResult(BaseActor actor, RequestPlayer message)
    {
        var player = actor as PlayerActor;
        switch (message.Opcode)
        {
            case 10000:
                return await HomeLoginHandler.LoginKeyHandler(player,
                    SerializeHelper.FromBinary<AHPlayerLoginKeyAsk>(message.Content));
        }

        A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        return null;
    }

    public async Task DispatcherNoResult(BaseActor actor, RequestPlayer message)
    {
        var player = actor as PlayerActor;
        switch (message.Opcode)
        {
        }

        A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
    }
}