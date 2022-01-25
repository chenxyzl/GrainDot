using System.Threading.Tasks;
using Home.Model.Component;

namespace Home.Hotfix.Service;

public static class PlayerService
{
    public static Task Load(this PlayerComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Start(this PlayerComponent self, bool crossDay)
    {
        return Task.CompletedTask;
    }

    public static Task PreStop(this PlayerComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Stop(this PlayerComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Online(this PlayerComponent self, bool newLogin, long lastLogoutTime)
    {
        return Task.CompletedTask;
    }

    public static Task Offline(this PlayerComponent self)
    {
        return Task.CompletedTask;
    }
}