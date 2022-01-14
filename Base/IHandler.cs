using Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base
{
    [Rpc]
    public interface IHandler
    {
        //在这里注册
        Dictionary<uint, RpcHandler<REQ, RSQ>> GetRpcHandler<REQ, RSQ>() where REQ : IRequest where RSQ : IResponse;
        Dictionary<uint, RnHandler<MSG>> GetRnHandler<MSG>() where MSG : IMessage;
    }
}