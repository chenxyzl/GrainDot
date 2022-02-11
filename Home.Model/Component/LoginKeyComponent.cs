using System;
using System.Collections.Generic;
using Akka.Actor;
using Base;

namespace Home.Model.Component;

public class LoginKeyComponent : IGlobalComponent
{
    public readonly object lockObj = new();
    public readonly Dictionary<string, IActorRef> loginKeys = new(); //key 对应的 actor
    public readonly Random random = new();
    public readonly SortedDictionary<long, string> timeKeys = new();
}