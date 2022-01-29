using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Share.Model.Component;

public class HttpComponent : IGlobalComponent
{
    public readonly string Addr;
    public IWebHost Host;

    public HttpComponent(string addr)
    {
        Addr = addr;
    }
}