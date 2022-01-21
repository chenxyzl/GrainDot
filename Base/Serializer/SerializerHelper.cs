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

        public static byte[] ToBinary<T>(this T obj) where T : class, IMessage
        {
            return ToBinary(obj);
        }

        public static T FromBinary<T>(byte[] bytes) where T : class, IMessage
        {
            using (var stream = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize(typeof(T), stream) as T;
            }
        }
        public static IMessage FromBinary(Type type, byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize(type, stream) as IMessage;
            }
        }
    }
}
