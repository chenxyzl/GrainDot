using Base.CustomAttribute.GameLife;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base
{
    public class GameHotfixManager : Single<GameHotfixManager>
    {
        private IGameHotfixLife hotfix;
        public IGameHotfixLife Hotfix
        {
            get { return A.RequireNotNull(hotfix, Code.Error, $"{this.GetType().FullName} must not null"); ; }
        }
        public void ReloadHanlder()
        {
            HashSet<Type> types = HotfixManager.Instance.GetTypes<GameServiceAttribute>();
            if (types.Count == 0)
            {
                GlobalLog.Warning($"{this.GetType().FullName} hotfix success but no type changed");
                return;
            }
            if (types.Count > 1)
            {
                A.Abort(Code.Error, $"GameLife.ServiceAttribute Count:{types.Count} Error");
            }
            hotfix = Activator.CreateInstance(types.First()) as IGameHotfixLife;
        }
    }
}
