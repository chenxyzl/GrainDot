using Base;
using Base.Alg;
using Home.Model;
using NLog;
using NLog.Config;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Boot
{
    class Program
    {
        static readonly UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();
        static async Task Main(string[] args)
        {

            CancellationTokenSource CancellationTokenSource;
            var a = new World.Model.World();
            var b = new Home.Model.Home();
            a.test();
            b.test();
            var c = new TestModel();
            CancellationTokenSource = new CancellationTokenSource();

            while (true)
            {
                try
                {
                    string line = await Task.Factory.StartNew(() =>
                    {
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
                    var asm = Base.Helper.DllHelper.GetHotfixAssembly();
                    types.Clear();
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
                    Console.WriteLine("load");
                    foreach (Type type in types[typeof(ANHandlerAttribute)])
                    {
                        IHandler obj = Activator.CreateInstance(type) as IHandler;
                        if (obj == null)
                        {
                            throw new Exception($"type not is AEvent: {obj.GetType().Name}");
                        }
                        obj.test(c);
                    }

                    foreach (Type type in types[typeof(ANServiceAttribute)])
                    {
                        type.GetMethod("test", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
                    }

                }
                catch
                {
                    Console.WriteLine("xxxxx");
                }
            }
        }
    }
}
