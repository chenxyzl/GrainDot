using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message
{
    public enum OpType
    {
        RR = 1, // 请求-应答
        R = 2, //请求-
        P = 3, //推送-
    }
}
