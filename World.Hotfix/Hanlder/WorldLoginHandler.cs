using System.Threading.Tasks;
using Message;
using World.Model;

namespace World.Hotfix.Handler;

public static class WorldLoginHandler
{
    public static Task<WHPlayerOnlineAns> Login(WorldSession player, HWPlayerOnlineAsk ask)
    {
        return Task.FromResult(new WHPlayerOnlineAns());
    }

    public static Task<WHPlayerOfflineAns> Offline(WorldSession player, HWPlayerOfflineAsk ask)
    {
        return Task.FromResult(new WHPlayerOfflineAns());
    }
}