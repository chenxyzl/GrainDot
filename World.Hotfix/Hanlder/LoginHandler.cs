using Base.Helper;
using Message;
using System.Threading.Tasks;
using World.Model;

namespace World.Hotfix.Handler
{
    public static class LoginHandler
    {

        public static Task<WHPlayerOnlineAns> Login(WorldSession player, HWPlayerOnlineAsk ask)
        {
            return Task.FromResult(new WHPlayerOnlineAns { });
        }
        public static Task<WHPlayerOfflineAns> Offline(WorldSession player, HWPlayerOfflineAsk ask)
        {
            return Task.FromResult(new WHPlayerOfflineAns { });
        }
    }
}
