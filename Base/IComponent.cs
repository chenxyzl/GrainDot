using System.Threading.Tasks;
using Message;

namespace Base;

public interface IInnerHandlerDispatcher
{
    public Task Dispatcher(BaseActor actor, IRequest message);
}

public interface IGateHandlerDispatcher
{
    public Task Dispatcher(BaseActor actor, Request message);
}