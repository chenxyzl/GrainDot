using Base;
using Base.Alg;
using Base.Helper;
using Home.Model;
using NLog;
using NLog.Config;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Boot
{
    class Test
    {
        static readonly UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();
        static async Task Main(string[] args)
        {

            CancellationTokenSource CancellationTokenSource;
            var a = new TestModel();
            CancellationTokenSource = new CancellationTokenSource();

            while (true)
            {
                try
                {
                    string line = await Task.Factory.StartNew(() =>
                    {
                        GlobalLog.Warning("按回车测试热更新");
                        return Console.In.ReadLine();
                    }, CancellationTokenSource.Token);

                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();

                    if (line == "0")
                    {
                        return;
                    }
                    types.Clear();
                    var asm = Base.Helper.DllHelper.GetHotfixAssembly();
                    foreach (var x in asm)
                    {
                        foreach (var type in x.GetTypes())
                        {
                            if (type.IsAbstract)
                            {
                                continue;
                            }

                            object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), true);
                            if (objects.Length == 0)
                            {
                                continue;
                            }

                            foreach (BaseAttribute baseAttribute in objects)
                            {
                                types.Add(baseAttribute.AttributeType, type);
                            }
                        }
                    }
                    GlobalLog.Info("load");
                    foreach (Type type in types[typeof(ANHandlerAttribute)])
                    {
                        IHandler obj = Activator.CreateInstance(type) as IHandler;
                        if (obj == null)
                        {
                            throw new Exception($"type not is AEvent: {obj.GetType().Name}");
                        }
                        obj.test(a);

                        //性能测试
                        int x = 1;
                        long xx = TimeHelper.NowNano();
                        for (int i = 1; i < 999999999; i++)
                        {
                            x = obj.a(x);
                        }
                        xx = TimeHelper.NowNano() - xx;
                        GlobalLog.Warning("性能测试1:" + x.ToString() + "  " + xx.ToString());
                        //性能测试
                        obj.test2(a);
                    }

                    foreach (Type type in types[typeof(ANServiceAttribute)])
                    {
                        int x = 1;
                        long xx = TimeHelper.NowNano();
                        for (int i = 1; i < 99999999; i++)
                        {
                            type.GetMethod("test", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
                        }
                        xx = TimeHelper.NowNano() - xx;
                        GlobalLog.Warning("性能测试3:" + x.ToString() + "  " + xx.ToString());
                        
                    }

                }
                catch
                {
                    GlobalLog.Error("error");
                }
            }
        }
    }
}
