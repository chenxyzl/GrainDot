using Base.Helper;
using Message;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Base.Network.Http
{
    public class HttpComponent : IGlobalComponent
    {

        IWebHost _server;
        string _addr;
        public HttpComponent(string addr) { _addr = addr; }

        public Task Load()
        {
            return Task.CompletedTask;
        }


        public async Task Start()
        {
            await Start(_addr);
        }

        public async Task PreStop()
        {
            await _server.StopAsync();
        }
        public Task Stop()
        {
            return Task.CompletedTask;
        }

        public Task Tick()
        {
            //tick里检查所有的链接是否有超时未登陆的 如果有则关闭链接
            return Task.CompletedTask;
        }

        async Task Start(string address)
        {
            var a = address.Split(":");
            var ip = a.Length > 1 && a[0] != "" ? IPAddress.Parse(a[0]) : IPAddress.Any;
            var port = a.Length > 1 ? int.Parse(a[1]) : int.Parse(a[0]);
            _server = new WebHostBuilder().UseKestrel((options) =>
            {
                options.Listen(ip, port, listenOptions => { });
            }).Configure(app =>
            {
                app.Run(ProcessAsync);
            }).Build();

            Console.WriteLine("http begin at:1988");
            await _server.StartAsync();
        }

        async Task ProcessAsync(HttpContext context)
        {
            try
            {
                StreamReader reader = new StreamReader(context.Request.Body);
                string base64 = reader.ReadToEnd().Replace("_", "=");
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
}
