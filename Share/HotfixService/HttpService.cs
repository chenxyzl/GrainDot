using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Base;
using Base.Serialize;
using Message;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Share.Model.Component;

namespace Share.Hotfix.Service;

[Service(typeof(HttpComponent))]
public static class HttpService
{
    public static async Task Start(this HttpComponent self)
    {
        var a = self.Addr.Split(":");
        var ip = a.Length > 1 && a[0] != "" ? IPAddress.Parse(a[0]) : IPAddress.Any;
        var port = a.Length > 1 ? int.Parse(a[1]) : int.Parse(a[0]);
        self.Host = new WebHostBuilder().UseKestrel(options => { options.Listen(ip, port, listenOptions => { }); })
            .Configure(app => { app.Run(self.ProcessAsync); }).Build();

        GlobalLog.Warning($"http begin at:{ip}:{port}");
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
            context.Response.Headers.Add("Date", new DateTimeOffset().UtcDateTime.ToString("u"));
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
            GlobalLog.Debug($"Path:{context.Request.Path} ret:{e.Code} e:{e.Message}");
        }
        catch (Exception e)
        {
            await context.Response.WriteAsync($"server inner error,{e}");
            await context.Response.Body.FlushAsync();
            GlobalLog.Debug($"Path:{context.Request.Path} unexpect exception");
        }
    }
    
    public static Task Load(this HttpComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Stop(this HttpComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Tick(this HttpComponent self, long now)
    {
        return Task.CompletedTask;
    }
}