using System;
using System.Net.Http;
using System.Threading.Tasks;
using Base;
using Base.Serialize;
using Message;

namespace Robot;

public static class Http
{
    public static async Task<byte[]> Request(string url, IMessage req)
    {
        //请求路径
        // string url = "http://127.0.0.1:20001/api/login";
        //定义request并设置request的路径
        var data = Convert.ToBase64String(req.ToBinary());

        var client = new HttpClient();
        var result = await client.PutAsync(url, new StringContent(data));
        var resBase64String = result.Content.ReadAsStringAsync().Result;

        var res = Convert.FromBase64String(resBase64String);
        var msg = SerializeHelper.FromBinary<ApiResult>(res);
        if (msg.Code != Code.Ok) throw new CodeException(msg.Code, $"get code {msg.Code}");

        return msg.Content;
    }
}