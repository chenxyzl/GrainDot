using System.IO;
using ProtoBuf;

namespace Base.Helper;

public static class ProtobufHelper
{
    public static void ToStream(object message, MemoryStream stream)
    {
        Serializer.Serialize(stream, message);
    }

    public static byte[] ToBytes(object message)
    {
        var stream = new MemoryStream();
        Serializer.Serialize(stream, message);
        return stream.ToArray();
    }

    public static object FromBytes(Type type, byte[] bytes)
    {
        using (var ms = new MemoryStream(bytes))
        {
            return Serializer.Deserialize(type, ms);
        }
    }

    public static T FromBytes<T>(byte[] msg)
    {
        var result = default(T);
        using (var ms = new MemoryStream())
        {
            //将消息写入流中
            ms.Write(msg, 0, msg.Length);
            //将流的位置归0
            ms.Position = 0;
            //使用工具反序列化对象
            result = Serializer.Deserialize<T>(ms);
            return result;
        }


        //using (MemoryStream ms = new MemoryStream(bytes))
        //{
        //    return (I)ProtoBuf.Serializer.Deserialize(typeof(I), ms);
        //}
    }
}