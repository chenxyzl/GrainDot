using Base.CustomAttribute.PlayerLife;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base
{
    public class PlayerHotfixManager : Single<PlayerHotfixManager>
    {
        private IPlayerHotfixLife hotfix;
        public IPlayerHotfixLife Hotfix
        {
            get { return A.RequireNotNull(hotfix, Code.Error, $"{this.GetType().FullName} must not null"); ; }
        }
        public void ReloadHanlder()
        {
            HashSet<Type> types = AttrManager.Instance.GetTypes<PlayerServiceAttribute>();
            if (types.Count == 0)
            {
                GlobalLog.Warning($"{this.GetType().FullName} hotfix success but no type changed");
                return;
            }
            if (types.Count > 1)
            {
                A.Abort(Code.Error, $"PlayerLife.ServiceAttribute Count:{types.Count} Error");
            }
            hotfix = Activator.CreateInstance(types.First()) as IPlayerHotfixLife;
        }
    }
}
