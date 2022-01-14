using Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base
{
    [Rpc]
    public interface IHandler
    {

    }

    public interface IInnerHandler
    {
        public void InnerHandleRequest(BaseActor actor, InnerRequest message);
        public void InnerHandleResponse(BaseActor actor, InnerRequest message);
    }

    public interface IGateHandler
    {
        public void IGateHandle(BaseActor actor, Response message);
    }
}