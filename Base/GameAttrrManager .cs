
using Base.Alg;
using Base.CustomAttribute.GameLife;
using Common;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public class GameAttrManager : Single<GameAttrManager>
    {
        //这里是申明为多个 实际为最多1个 预留给未来扩展
        private Dictionary<Type, MethodInfo> loadDelegtes = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> startDelegtes = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> preStopDelegtes = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> stopHandlerDelegtes = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> onlineDelegtes = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> offlineDelegtes = new Dictionary<Type, MethodInfo>();
        private Dictionary<Type, MethodInfo> tickDelegtes = new Dictionary<Type, MethodInfo>();
        public void ReloadHanlder()
        {
            Dictionary<Type, MethodInfo> loadDelegtesTemp = new Dictionary<Type, MethodInfo>();
            Dictionary<Type, MethodInfo> startDelegtesTemp = new Dictionary<Type, MethodInfo>();
            Dictionary<Type, MethodInfo> preStopDelegtesTemp = new Dictionary<Type, MethodInfo>();
            Dictionary<Type, MethodInfo> stopHandlerDelegtesTemp = new Dictionary<Type, MethodInfo>();
            Dictionary<Type, MethodInfo> onlineDelegtesTemp = new Dictionary<Type, MethodInfo>();
            Dictionary<Type, MethodInfo> offlineDelegtesTemp = new Dictionary<Type, MethodInfo>();
            Dictionary<Type, MethodInfo> tickDelegtesTemp = new Dictionary<Type, MethodInfo>();
            HashSet<Type> types = AttrManager.Instance.GetTypes<GameServiceAttribute>();
            if (types.Count > 1)
            {
                A.Abort(Code.Error, $"PlayerLife.ServiceAttribute Count:{types.Count} Error");
            }
            foreach (var type in types)
            {
                //所有所有handler方法
                var methods = type.GetMethods();
                foreach (var metchod in methods)
                {
                    GameLoadAttribute loadDelegteAttr = metchod.GetCustomAttribute<GameLoadAttribute>(true);
                    if (loadDelegteAttr != null)
                    {
                        loadDelegtesTemp[metchod.GetParameters()[0].ParameterType] = metchod;
                        continue;
                    }
                    GameStartAttribute startDelegteAttr = metchod.GetCustomAttribute<GameStartAttribute>(true);
                    if (startDelegteAttr != null)
                    {
                        startDelegtesTemp[metchod.GetParameters()[0].ParameterType] = metchod;
                        continue;
                    }
                    GamePreStopAttribute preStopDelegteAttr = metchod.GetCustomAttribute<GamePreStopAttribute>(true);
                    if (preStopDelegteAttr != null)
                    {
                        preStopDelegtesTemp[metchod.GetParameters()[0].ParameterType] = metchod;
                        continue;
                    }
                    GameStopAttribute stopHandlerDelegteAttr = metchod.GetCustomAttribute<GameStopAttribute>(true);
                    if (stopHandlerDelegteAttr != null)
                    {
                        stopHandlerDelegtesTemp[metchod.GetParameters()[0].ParameterType] = metchod;
                        continue;
                    }
                    GameOnlineAttribute onlineDelegteAttr = metchod.GetCustomAttribute<GameOnlineAttribute>(true);
                    if (onlineDelegteAttr != null)
                    {
                        onlineDelegtesTemp[metchod.GetParameters()[0].ParameterType] = metchod;
                        continue;
                    }
                    GameOfflineAttribute offlineDelegteAttr = metchod.GetCustomAttribute<GameOfflineAttribute>(true);
                    if (offlineDelegteAttr != null)
                    {
                        offlineDelegtesTemp[metchod.GetParameters()[0].ParameterType] = metchod;
                        continue;
                    }
                    GameTickAttribute tickDelegteAttr = metchod.GetCustomAttribute<GameTickAttribute>(true);
                    if (tickDelegteAttr != null)
                    {
                        tickDelegtesTemp[metchod.GetParameters()[0].ParameterType] = metchod;
                        continue;
                    }

                }
            }
            (loadDelegtes, loadDelegtesTemp) = (loadDelegtesTemp, loadDelegtes);
            loadDelegtesTemp.Clear();
            (startDelegtes, startDelegtesTemp) = (startDelegtesTemp, startDelegtes);
            startDelegtesTemp.Clear();
            (preStopDelegtes, preStopDelegtesTemp) = (preStopDelegtesTemp, preStopDelegtes);
            preStopDelegtesTemp.Clear();
            (stopHandlerDelegtes, stopHandlerDelegtesTemp) = (stopHandlerDelegtesTemp, stopHandlerDelegtes);
            stopHandlerDelegtesTemp.Clear();
            (onlineDelegtes, onlineDelegtesTemp) = (onlineDelegtesTemp, onlineDelegtes);
            onlineDelegtesTemp.Clear();
            (offlineDelegtes, offlineDelegtesTemp) = (offlineDelegtesTemp, offlineDelegtes);
            offlineDelegtesTemp.Clear();
            (tickDelegtes, tickDelegtesTemp) = (tickDelegtesTemp, tickDelegtes);
            tickDelegtesTemp.Clear();
        }
    }
}
