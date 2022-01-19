
using Base.Alg;
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

    public class Config : Single<Config>
    {
        private Dictionary<Type, ACategory> configs = new Dictionary<Type, ACategory>();
        public void ReloadConfig()
        {
            Dictionary<Type, ACategory> newDic = new Dictionary<Type, ACategory>();
            HashSet<Type> types = HotfixManager.Instance.GetTypes<ConfigAttribute>();
            foreach (var type in types)
            {
                object obj = Activator.CreateInstance(type);

                ACategory iCategory = obj as ACategory;
                if (iCategory == null)
                {
                    throw new Exception($"class: {type.Name} not inherit from ACategory");
                }
                iCategory.BeginInit();
                iCategory.EndInit();
                newDic[iCategory.ConfigType] = iCategory;
            }
            (configs, newDic) = (newDic, configs);
            newDic.Clear();
        }

        public T Get<T>() where T : ACategory
        {
            return (T)configs[typeof(T)];
        }
    }
}
