using Message;

namespace Base;

public interface IInnerHandlerDispatcher
{
    public void Dispatcher(BaseActor actor, InnerRequest message)
    {
    }
}

public interface IGateHandlerDispatcher
{
    public void Dispatcher(BaseActor actor, Request message)
    {
    }
}