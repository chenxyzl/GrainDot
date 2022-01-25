using System.Linq;
using Base.CustomAttribute.GameLife;
using Message;

namespace Base;

public class GameHotfixManager : Single<GameHotfixManager>
{
    private IGameHotfixLife hotfix;

    public IGameHotfixLife Hotfix => A.RequireNotNull(hotfix, Code.Error, $"{GetType().FullName} must not null");

    public void ReloadHandler()
    {
        var types = HotfixManager.Instance.GetTypes<GameServiceAttribute>();
        if (types.Count == 0)
        {
            GlobalLog.Warning($"{GetType().FullName} hotfix success but no type changed");
            return;
        }

        if (types.Count > 1) A.Abort(Code.Error, $"GameLife.ServiceAttribute Count:{types.Count} Error");

        hotfix = Activator.CreateInstance(types.First()) as IGameHotfixLife;
    }
}