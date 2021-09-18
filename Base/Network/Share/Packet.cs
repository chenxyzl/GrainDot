using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Network.Share
{
    public class Packet
    {
        private MemoryStream _stream;
        public readonly int Size;
        public Packet(int size)
        {
            Size = size;
            A.Ensure(size <= NetDefine.SizeLen && size >= 0, PB.Code.Error, $"packet lengh error:{size}");
            _stream = new MemoryStream(size);
        }
        //写入数据
        public void WriteBuff(byte[] b, int offset, int lengh)
        {
            _stream.Write(b, offset, lengh);
        }
        //获取完整数据的steam
        public MemoryStream GetCompleteStream()
        {
            A.Ensure(_stream.Length == Size, PB.Code.Error, $"Size:{Size} not == stream lengh:{_stream.Length}");
            return _stream;
        }

        public int CurrentLengh()
        {
            return (int)_stream.Length;
        }
    }
}
