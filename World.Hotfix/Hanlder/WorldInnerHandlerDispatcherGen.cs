using System;
using System.Threading.Tasks;
using Base;
using Message;
using World.Model;

namespace Home.Hotfix.Handler;

public partial class WorldInnerHandlerDispatcher
{
    public Task<IResponse> DispatcherWithResult(WorldSession player, RequestWorld message)
    {
        A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        //不会执行导致，只是解决编译错误
        throw new Exception();
    }

    public Task DispatcherNoResult(WorldSession player, RequestWorld message)
    {
        A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        return Task.CompletedTask;
    }
}