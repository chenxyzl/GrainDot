using System.Collections.Generic;
using Base.ConfigParse;

namespace Base;

public class ConfigManager : Single<ConfigManager>
{
    private Dictionary<Type, ACategory> configs = new();

    public void ReloadConfig()
    {
        var newDic = new Dictionary<Type, ACategory>();
        var types = HotfixManager.Instance.GetTypes<ConfigAttribute>();
        foreach (var type in types)
        {
            var obj = Activator.CreateInstance(type);

            var iCategory = obj as ACategory;
            if (iCategory == null) throw new Exception($"class: {type.Name} not inherit from ACategory");

            iCategory.BeginInit();
            iCategory.EndInit();
            newDic[iCategory.GetType()] = iCategory;
        }

        (configs, newDic) = (newDic, configs);
        newDic.Clear();
    }

    public T Get<T>() where T : ACategory
    {
        return (T) configs[typeof(T)];
    }
}