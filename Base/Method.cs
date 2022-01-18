using Base;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public static class ExtensionMethods
{
    //反射调用异步方法
    public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
    {
        dynamic awaitable = @this.Invoke(obj, parameters);
        await awaitable;
        return awaitable.GetAwaiter().GetResult();
    }

    //获取方法的扩展方法  包括继承
    public static IEnumerable<MethodInfo> GetExtensionMethods(this Assembly assembly, Type extendedType)
    {
        var query = from type in assembly.GetTypes()
                    where type.IsAbstract && type.IsSealed
                    from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                    where method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                    where method.GetParameters()[0].ParameterType.IsAssignableFrom(extendedType)
                    select method;
        return query;
    }

    //public static IEnumerable<MethodInfo> GetRpcMethods(this Assembly assembly)
    //{
    //    var query = from type in assembly.GetTypes()
    //                where type.IsAssignableFrom(typeof(IHandler))
    //                from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
    //                where method.GetCustomAttributes(typeof(RpcHandlerAttribute), true).Length > 0
    //                select method;
    //    return query;
    //}
}