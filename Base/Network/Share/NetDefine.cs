using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Network.Share
{
    public static class NetDefine
    {
        //标识长度的字节个数 //不要动
        public static readonly int SizeLen = 4; //
        //最大消息长度 过大的消息最好做拆分处理 不要动
        public static readonly ushort MaxSize = ushort.MaxValue;
    }
}
