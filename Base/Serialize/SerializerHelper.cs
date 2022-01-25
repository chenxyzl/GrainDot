using System.IO;
using Message;
using ProtoBuf;

namespace Base.Serialize;

public static class SerializeHelper
{
    public static byte[] ToBinary(object obj)
    {
        using (var stream = new MemoryStream())
        {
            Serializer.Serialize(stream, obj);
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
            return Serializer.Deserialize(typeof(T), stream) as T;
        }
    }

    public static IMessage FromBinary(Type type, byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        {
            return Serializer.Deserialize(type, stream) as IMessage;
        }
    }
}