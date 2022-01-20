using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.ET;
using Base.Helper;
using Base.Serializer;
using Common;
using Message;
using Share.Model.Component;

namespace Share.Hotfix.Service
{
    public static class CallService
    {
        public static async ETTask<IResponse> Call(this BaseActor self, IActorRef other, IRequest request)
        {
            //request转id
            var tcs = ETTask<IResponse>.Create(true);
            var callComponent = self.GetComponent<CallComponent>();
            var rid = callComponent.AddRequestCallBack(new SenderMessage(TimeHelper.Now(), tcs));
            //
            InnerRequest innerRequest = new InnerRequest { Opcode = 1, Content = request.ToBinary(), Sn = rid };
            other.Tell(innerRequest);
            //
            long beginTime = TimeHelper.Now();
            IResponse response = await tcs;
            long cost = TimeHelper.Now() - beginTime;
            //
            if (cost >= 100)
            {
                self.Logger.Warning($"call cost time:{cost} too long");
            }
            //
            return response;
        }

        public static void Notify(this BaseActor self, IActorRef other, IRequest request)
        {
            InnerRequest innerRequest = new InnerRequest { Opcode = 1, Content = request.ToBinary(), Sn = 0 };
            other.Tell(innerRequest);
        }

        public static Task Tick(this CallComponent self, long dt)
        {
            var dic = self.RequestCallbackDic;
            var now = TimeHelper.Now();
            while (true)
            {
                if (dic.Count == 0)
                {
                    break;
                }
                var first = dic.First();
                if (now - first.Value.CreateTime < GlobalParam.RPC_TIMEOUT_TIME)
                {
                    break;
                }
                //todo 超时调用
            }
            return Task.CompletedTask;
        }
    }
}
