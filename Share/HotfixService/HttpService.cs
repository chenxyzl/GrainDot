using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class HttpService
{
    static public async Task Start(this HttpComponent self)
    {
        var a = self.Addr.Split(":");
        var ip = a.Length > 1 && a[0] != "" ? IPAddress.Parse(a[0]) : IPAddress.Any;
        var port = a.Length > 1 ? int.Parse(a[1]) : int.Parse(a[0]);
        self.Host = new WebHostBuilder().UseKestrel(options => { options.Listen(ip, port, listenOptions => { }); })
            .Configure(app => { app.Run(self.ProcessAsync); }).Build();

        Console.WriteLine("http begin at:1988");
        await self.Host.StartAsync();
    }

    static public async Task PreStop(this HttpComponent self)
    {
        await self.Host.StopAsync();
    }

    static private async Task ProcessAsync(this HttpComponent self, HttpContext context)
    {
        try
        {
            var reader = new StreamReader(context.Request.Body);
            var base64 = reader.ReadToEnd().Replace("_", "=");
            var data = Convert.FromBase64String(base64);
            var handler = HttpHotfixManager.Instance.GetHandler(context.Request.Path);
            var result = await handler.Handle(data);
            var ret = Convert.ToBase64String(result);
            await context.Response.WriteAsync(ret);
        }
        catch (Exception e)
        {
            await context.Response.WriteAsync($"hello:{context.Request.Path}: {e.ToString()}");
        }
    }
}