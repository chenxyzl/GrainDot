using System;
using System.Threading.Tasks;
using Base;
using Base.Helper;
using Base.Serialize;
using Message;
using Newtonsoft.Json;

namespace Robot;

public class Client
{
    private NetClient _netClient = null!;
    private long lastSend = TimeHelper.Now();
    private int R = 10_000;

    public async Task<ulong> Login()
    {
        R = (RandomHelper.RandInt32() % 20 + 10) * 1000;
        var uid = IdGenerater.NextId();
        try
        {
            //http登录
            var data = await Http.Request("http://10.7.69.254:20001/api/login",
                new C2ARoleLogin {Uid = uid, Token = RandomHelper.RandUInt64().ToString()});
            var res = SerializeHelper.FromBinary<A2CRoleLogin>(data);
            //链接tcp
            var ip = res.Addr.Split(":")[0];
            if (ip == "0.0.0.0") ip = "10.7.69.254";

            var port = ushort.Parse(res.Addr.Split(":")[1]);
            _netClient = new NetClient(ip, port)
            {
                onRecMsg = OnRecMsg
            };
            //tcp登录
            _netClient.Send(new C2SLogin
            {
                PlayerId = uid,
                Key = res.Key
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }

        return uid;
    }

    public void Iick(long now)
    {
        _netClient.ReceiveMsg();
        // return;
        if (now > lastSend + R)
        {
            获取邮件();
            lastSend = now;
        }
    }

    private void 获取邮件()
    {
        _netClient.Send(new C2SMails());
    }

    void OnRecMsg(byte[] msg)
    {
        //第1层反序列化
        var rsp = SerializeHelper.FromBinary<Response>(msg);
        //第2层反序列化
        var type = RpcManager.Instance.GetResponseOpcode(rsp.Opcode);
        var ret = SerializeHelper.FromBinary(type, rsp.Content ?? new byte[] { });
        //显示
        var recMes = ret.ToString()!;
        GlobalLog.Debug(recMes);
    }
}