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
    public class Http
    {
        Dictionary<string, Action> Router;
        void Start(string address, string tag)
        {
            ParaseRouter(tag);
            var a = address.Split(":");
            var ip = a.Length > 1 && a[0] != "" ? IPAddress.Parse(a[0]) : IPAddress.Any;
            var port = a.Length > 1 ? int.Parse(a[1]) : int.Parse(a[0]);
            var host = new WebHostBuilder().UseKestrel((options) =>
            {
                options.Listen(ip, port, listenOptions => { });
            }).Configure(app =>
            {
                app.Run(ProcessAsync);
            }).Build();

            Console.WriteLine("http begin at:1988");
            host.Start();
        }

        void ParaseRouter(string tag)
        {
            var r = new Dictionary<string, AMRpcHandler>();
            var handlers = AttrManager.Instance.GetTypes<HttpMethodAttribute>();
            foreach (var h in handlers)
            {
                var attrs = h.GetCustomAttributes(typeof(HttpMethodAttribute), true) as HttpMethodAttribute[];
                foreach (var a in attrs)
                {
                    if (a.Tag == tag)
                    {

                    }
                }
            }
        }

        async Task ProcessAsync(HttpContext context)
        {
            try
            {
                StreamReader reader = new StreamReader(context.Request.Body);
                string base64 = reader.ReadToEnd().Replace("_", "=");
                switch (context.Request.Path)
                {
                    case "/api/login":
                        var data = Convert.FromBase64String(base64);
                        var request = ProtobufHelper.FromBytes<C2R_Login>(data);
                        var result = await Login(request);
                        Console.WriteLine(request);

                        await context.Response.WriteAsync(result);
                        break;
                    default:
                        await context.Response.WriteAsync("hello");
                        break;
                }
            }
            catch (Exception e)
            {
                await context.Response.WriteAsync($"hello:{context.Request.Path}: {e.ToString()}");
            }
        }
    }
}
