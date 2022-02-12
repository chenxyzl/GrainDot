using System.Threading.Tasks;
using Base;
using Base.Serialize;
using Home.Model;
using Message;

namespace Home.Hotfix.Handler;

public partial class GateHandlerDispatcher
{
    public async Task<IResponse> DispatcherWithResult(PlayerActor player, Request message)
    {
        switch (message.Opcode)
        {
            case 200000:
                return await HomeLoginHandler.Ping(player, SerializeHelper.FromBinary<C2SPing>(message.Content));
            case 200003:
                return await HomeLoginHandler.Login(player, SerializeHelper.FromBinary<C2SLogin>(message.Content));
        }

        A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        return null;
    }

    public async Task DispatcherNoResult(PlayerActor player, Request message)
    {
        switch (message.Opcode)
        {
            case 200001:
                await HomeLoginHandler.NotifyTest(player, SerializeHelper.FromBinary<CNotifyTest>(message.Content));
                break;
        }

        A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
    }
}