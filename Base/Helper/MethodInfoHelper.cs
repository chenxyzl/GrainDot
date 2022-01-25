﻿using System.Reflection;

namespace Base.Helper;

public static class MethodInfoHelper
{
    public static void Run(this MethodInfo methodInfo, object obj, params object[] param)
    {
        if (methodInfo.IsStatic)
        {
            var p = new object[param.Length + 1];
            p[0] = obj;
            for (var i = 0; i < param.Length; ++i) p[i + 1] = param[i];

            methodInfo.Invoke(null, p);
        }
        else
        {
            methodInfo.Invoke(obj, param);
        }
    }
}