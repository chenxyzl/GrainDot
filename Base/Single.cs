using System;

public class Single<T> where T : class
{
    private static T _instance;
    private static readonly object _syncLock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //加锁
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        //Activator.CreateInstance()  创建类,获取类的实例
                        _instance = Activator.CreateInstance(typeof(T), true) as T;
                    }
                    return _instance;
                }
            }
            else
            {
                return _instance;
            }
        }
    }
}