using System;
using System.Linq;
using System.Net.Sockets;
using Base;
using Base.Helper;
using Base.Serialize;
using Message;

namespace Robot;

/// <summary>
///     封装Socket
/// </summary>
public class NetClient
{
    public delegate void OnRevMsg(byte[] msg);

    private TcpClient? client;
    private bool isHead;
    private int len;

    public OnRevMsg? onRecMsg;
    private uint sn;

    public NetClient(string ip, int port)
    {
        client = new TcpClient();
        client.Connect(ip, port);
        isHead = true;
    }

    /// <summary>
    ///     发送消息
    /// </summary>
    private void send(byte[] msg)
    {
        if (client == null || !client.Connected) return;

        //消息体结构：消息体长度+消息体
        var data = new byte[2 + msg.Length];
        IntToBytes((ushort) msg.Length).CopyTo(data, 0);
        msg.CopyTo(data, 2);
        try
        {
            client.GetStream().Write(data, 0, data.Length);
        }
        catch (Exception)
        {
            // Console.WriteLine(e);
        }
    }

    public void Send(IMessage msg)
    {
        //第一层序列化
        var opcode = RpcManager.Instance.GetRequestOpcode(msg.GetType());
        //第二层序列化
        var req = new Request
        {
            Opcode = opcode,
            Content = msg.ToBinary(),
            Sn = ++sn,
            Sign = ""
        };
        //发送
        send(req.ToBinary());
    }

    /// <summary>
    ///     接收消息
    /// </summary>
    public void ReceiveMsg()
    {
        if (client == null || !client.Connected) return;

        var stream = client.GetStream();
        if (!stream.CanRead) return;

        //读取消息体的长度
        if (isHead)
        {
            if (client.Available < 2) return;

            var lenByte = new byte[2];
            stream.Read(lenByte, 0, 2);
            len = BytesToInt(lenByte, 0);
            isHead = false;
        }

        //读取消息体内容
        if (!isHead)
        {
            if (client.Available < len) return;

            var msgByte = new byte[len];
            stream.Read(msgByte, 0, len);
            isHead = true;
            len = 0;
            if (onRecMsg != null)
                //处理消息
                onRecMsg(msgByte);
        }
    }

    /// <summary>
    ///     bytes转int
    /// </summary>
    /// <param name="data"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static int BytesToInt(byte[] data, int offset)
    {
        var a = data.SubArray(offset, 2).Reverse().ToArray();
        var b = BitConverter.ToUInt16(a, 0);
        return b;
    }


    /// <summary>
    ///     int 转 bytes
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static byte[] IntToBytes(ushort num)
    {
        var bytes = BitConverter.GetBytes(num).Reverse().ToArray();
        return bytes;
    }

    public void Close()
    {
        if (client != null)
        {
            client.Close();
            return;
        }

        client = null;
    }
}