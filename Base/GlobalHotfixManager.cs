using System.Linq;
using Base.CustomAttribute.GlobalLife;
using Message;

namespace Base;

public class GlobalHotfixManager : Single<GlobalHotfixManager>
{
    private IGlobalHotfixLife hotfix;

    public IGlobalHotfixLife Hotfix => A.RequireNotNull(hotfix, Code.Error, $"{GetType().FullName} must not null");

    public void ReloadHandler()
    {
        var types = HotfixManager.Instance.GetTypes<GlobalServiceAttribute>();
        if (types.Count == 0)
        {
            GlobalLog.Warning($"{GetType().FullName} hotfix success but no type changed");
            return;
        }

        if (types.Count > 1) A.Abort(Code.Error, $"GlobalLife.ServiceAttribute Count:{types.Count} Error");

        hotfix = Activator.CreateInstance(types.First()) as IGlobalHotfixLife;
    }
}