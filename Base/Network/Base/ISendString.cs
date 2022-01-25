﻿using System.Threading.Tasks;

namespace Base.Network;

/// <summary>
///     发送字符串
/// </summary>
public interface ISendString
{
    /// <summary>
    ///     发送字符串
    /// </summary>
    /// <param name="msgStr">字符串</param>
    /// <returns></returns>
    Task Send(string msgStr);
}