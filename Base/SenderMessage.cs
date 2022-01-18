using Base.ET;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public class SenderMessage
    {

        public long CreateTime { get; }
        public ETTask<IResponse> Tcs { get; }
    }
}
