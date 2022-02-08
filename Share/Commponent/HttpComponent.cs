using Base;
using Microsoft.AspNetCore.Hosting;

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