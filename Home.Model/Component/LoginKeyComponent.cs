using System;
using System.Collections.Generic;
using Akka.Actor;
using Base;

namespace Home.Model.Component;

public class LoginKeyComponent : IGlobalComponent
{
    public readonly object lockObj = new();
    public readonly Dictionary<string, ActorSelection> loginKeys = new(); //key 对应的 actor
    public readonly Random random = new();
    public readonly SortedDictionary<ulong, string> timeKeys = new(); //带时间戳的id 对应的 key
    public long _lastConsoleTime;
}