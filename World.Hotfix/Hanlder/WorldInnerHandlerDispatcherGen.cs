using System.Threading.Tasks;
using Base;
using Message;
using World.Model;

namespace Home.Hotfix.Handler;

public partial class WorldInnerHandlerDispatcher
{
    public async Task<IResponse> DispatcherWithResult(WorldSession player, RequestWorld message)
    {
        switch (message.Opcode)
        {
        }

        A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        return null;
    }

    public async Task DispatcherNoResult(WorldSession player, RequestWorld message)
    {
        switch (message.Opcode)
        {
        }

        A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
    }
}