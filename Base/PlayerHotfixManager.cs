using System.Linq;
using Base.CustomAttribute.PlayerLife;
using Message;

namespace Base;

public class PlayerHotfixManager : Single<PlayerHotfixManager>
{
    private IPlayerHotfixLife? hotfix;

    public IPlayerHotfixLife Hotfix => A.NotNull(hotfix, Code.Error, $"{GetType().FullName} must not null");

    public void ReloadHanlder()
    {
        var types = HotfixManager.Instance.GetTypes<PlayerServiceAttribute>();
        if (types.Count == 0)
        {
            GlobalLog.Warning($"{GetType().FullName} hotfix success but no type changed");
            return;
        }

        if (types.Count > 1) A.Abort(Code.Error, $"PlayerLife.ServiceAttribute Count:{types.Count} Error");

        hotfix = A.NotNull(Activator.CreateInstance(types.First()) as IPlayerHotfixLife, Code.Error);
    }
}