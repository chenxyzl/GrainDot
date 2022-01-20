using Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base
{
    [InnerRpc]
    public interface IInnerHandlerDispatcher
    {
        public void Dispatcher(BaseActor actor, InnerRequest message) { }
    }

    [OuterRpc]
    public interface IGateHandlerDispatcher
    {
        public void Dispatcher(BaseActor actor, Request message) { }
    }
}