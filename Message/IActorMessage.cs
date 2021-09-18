using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message
{
    // 不需要返回消息
    public interface IActorMessage : IMessage
    {
    }

    public interface IActorRequest : IActorMessage, IRequest
    {
    }

    public interface IActorResponse : IResponse
    {
    }
}
