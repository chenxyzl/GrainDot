using System.Threading.Tasks;
using Base;
using Base.CustomAttribute.GlobalLife;
using Base.Network.Http;

namespace OM.Hotfix.Service;

[GlobalService]
public class LoginGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
    }

    public async Task Load()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public Task PreStop()
    {
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        return Task.CompletedTask;
    }


    public Task Tick()
    {
        return Task.CompletedTask;
    }
}