using System.Threading.Tasks;
using Base;
using Base.CustomAttribute.GlobalLife;

namespace OM.Hotfix.Service;

[GlobalService]
public class OMGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
    }

    public Task Load()
    {
        return Task.CompletedTask;
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