using System;
using Akka.Actor;
using Base;
using Base.ET;
using Message;

namespace Share.Hotfix.Service
{
    public static class CallService
    {
        public static async ETTask<IResponse> Call(this BaseActor self, IActorRef other, IRequest request)
        {
        }
    }
}
