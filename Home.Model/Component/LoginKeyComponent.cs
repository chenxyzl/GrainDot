using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.Helper;

namespace Home.Model.Component;

public class LoginKeyComponent : IGlobalComponent
{
    public readonly object lockObj = new();
    public readonly Dictionary<string, IActorRef> loginKeys = new();
    public readonly Dictionary<IActorRef, string> loginRefs = new();
    public readonly Random random = new();
    public readonly SortedDictionary<long, string> timeKeys = new();
}