
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Robot
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Thread.Sleep(1_000);
            //await Test.Instance.StartAsync();
        }


    }

    //abstract class Base<T>
    //{
    //    public static ConcurrentDictionary<string, T> a = new ConcurrentDictionary<string, T>();
    //    public abstract Task<object> F();
    //}

    //class A : Base<int>
    //{
    //    Task<int> F()
    //    {
    //        return Task.FromResult(1);
    //    }
    //}

    //class B : Base<int>
    //{

    //}


    //public class Single<T> where T : class
    //{
    //    private static T _instance;
    //    private static readonly object _syncLock = new object();

    //    public static T Instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //            {
    //                //加锁
    //                lock (_syncLock)
    //                {
    //                    if (_instance == null)
    //                    {
    //                        //Activator.CreateInstance()  创建类,获取类的实例
    //                        _instance = Activator.CreateInstance(typeof(T), true) as T;
    //                    }
    //                    return _instance;
    //                }
    //            }
    //            else
    //            {
    //                return _instance;
    //            }
    //        }
    //    }
    //}

    //class Test : Single<Test>
    //{
    //    public async Task StartAsync()
    //    {
    //        A.a["x"] = 1;
    //        B.a["y"] = 2;
    //        B.a["x"] = 3;
    //        Console.WriteLine(A.a);
    //        Console.ReadLine();
    //    }
    //}
}
