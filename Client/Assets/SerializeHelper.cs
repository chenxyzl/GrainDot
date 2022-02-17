using System;
using System.IO;
using Message;
using ProtoBuf;

public static class SerializeHelper
{
    public static byte[] ToBinary<T>(this T obj) where T : class, IMessage
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, obj);
        return stream.ToArray();
    }

    public static T FromBinary<T>(byte[] bytes) where T : class, IMessage
    {
        using var stream = new MemoryStream(bytes);
        var res = Serializer.Deserialize(typeof(T), stream) as T;
        if (res == null)
        {
            throw new Exception("null deserialize");
        }

        return res;
    }

    public static IMessage FromBinary(Type type, byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        var res = Serializer.Deserialize(type, stream) as IMessage;
        if (res == null)
        {
            throw new Exception("null deserialize");
        }

        return res;
    }
}