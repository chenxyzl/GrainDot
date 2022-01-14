using Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Serializer
{
    public static class SerializerHelper
    {
        public static byte[] ToBinary(object obj)
        {
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public static T FromBinary<T>(byte[] bytes) where T : class
        {
            using (var stream = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize(typeof(T), stream) as T;
            }
        }
    }
}
