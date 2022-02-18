using System;
using System.ComponentModel;
using System.IO;
using Google.Protobuf;
using Message;

public static class SerializeHelper
{
    public static byte[] ToBinary<T>(this T obj) where T : class, IMessage
    {
        return obj.ToByteArray();
    }

    public static T FromBinary<T>(byte[] bytes) where T : class, IMessage
    {
        var parser = typeof(T).GetProperty("Parser").GetValue(null) as MessageParser;
        using var stream = new MemoryStream(bytes);
        var res = parser.ParseFrom(bytes) as T;
        if (res == null)
        {
            throw new Exception("null deserialize");
        }

        return res;
    }

    public static IMessage FromBinary(Type type, byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        var parser = type.GetProperty("Parser").GetValue(null) as MessageParser;
        var res = parser.ParseFrom(bytes);
        if (res == null)
        {
            throw new Exception("null deserialize");
        }

        return res;
    }
}