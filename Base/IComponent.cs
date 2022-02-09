using Message;

namespace Base;

public interface IInnerHandlerDispatcher
{
    public void Dispatcher(BaseActor actor, IRequest message)
    {
    }
}

public interface IGateHandlerDispatcher
{
    public void Dispatcher(BaseActor actor, Request message)
    {
    }
}