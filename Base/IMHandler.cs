using System;
using System.Threading.Tasks;

namespace Base
{
    public interface IMHandler
    {
        Task Handle(BaseActor session, uint opcode, uint sn, object message);
        Type GetInType();
        Type GetRetType();
    }
}