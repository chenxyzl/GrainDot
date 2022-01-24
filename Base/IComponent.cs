using Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base
{
    public interface IInnerHandlerDispatcher
    {
        public void Dispatcher(BaseActor actor, InnerRequest message) { }
    }

    public interface IGateHandlerDispatcher
    {
        public void Dispatcher(BaseActor actor, Request message) { }
    }
}