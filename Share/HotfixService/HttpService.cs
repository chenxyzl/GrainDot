using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Base;
using Base.Serialize;
using Message;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class HttpService
{
    public static async Task Start(this HttpComponent self)
    {
        var a = self.Addr.Split(":");
        var ip = a.Length > 1 && a[0] != "" ? IPAddress.Parse(a[0]) : IPAddress.Any;
        var port = a.Length > 1 ? int.Parse(a[1]) : int.Parse(a[0]);
        self.Host = new WebHostBuilder().UseKestrel(options => { options.Listen(ip, port, listenOptions => { }); })
            .Configure(app => { app.Run(self.ProcessAsync); }).Build();

        GlobalLog.Debug($"http begin at:{ip}:{port}");
        await self.Host.StartAsync();
    }

    public static async Task PreStop(this HttpComponent self)
    {
        await self.Host.StopAsync();
    }

    private static async Task ProcessAsync(this HttpComponent self, HttpContext context)
    {
        try
        {
            var reader = new StreamReader(context.Request.Body);
            var base64 = (await reader.ReadToEndAsync()).Replace("_", "=");
            var data = Convert.FromBase64String(base64);
            var handler = HttpHotfixManager.Instance.GetHandler(context.Request.Path);
            var result = await handler.Handle(data);
            var array = new ApiResult
            {
                Code = Code.Ok,
                Content = result
            }.ToBinary();
            var ret = Convert.ToBase64String(array);
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html";
            context.Response.Headers.Add("Date", new DateTimeOffset().UtcDateTime.ToString());
            await context.Response.WriteAsync(ret);
            await context.Response.Body.FlushAsync();
        }
        catch (CodeException e)
        {
            var array = new ApiResult
            {
                Code = e.Code,
                Msg = e.Message
            }.ToBinary();
            var ret = Convert.ToBase64String(array);
            await context.Response.WriteAsync(ret);
            await context.Response.Body.FlushAsync();
        }
        catch (Exception e)
        {
            await context.Response.WriteAsync($"server inner error,{e}");
        }
    }
}