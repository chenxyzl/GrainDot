using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.ET;
using Base.Helper;
using Base.Serializer;
using Common;
using Message;

namespace Share.Model.Component
{
    public class CallComponent : IPlayerComponent
    {
        private ulong _requestIncId = 0;
        private SortedDictionary<ulong, SenderMessage> requestCallbackDic = new SortedDictionary<ulong, SenderMessage>();
        public SortedDictionary<ulong, SenderMessage> RequestCallbackDic { get => requestCallbackDic; }
        public CallComponent(BaseActor node) : base(node) { }

        public ulong NextRequestId() { return ++_requestIncId; }

        public SenderMessage GetCallback(ulong requestId)
        {
            return requestCallbackDic[requestId];
        }
        public void RemoveRequestCallBack(ulong requestId)
        {
            requestCallbackDic.Remove(requestId);
        }
        public ulong AddRequestCallBack(SenderMessage senderMessage)
        {
            var rid = NextRequestId();
            requestCallbackDic[rid] = senderMessage;
            return rid;
        }
        public async ETTask<IResponse> Call(IActorRef other, IRequest request)
        {
            //request转id
            var tcs = ETTask<IResponse>.Create(true);
            var callComponent = Node.GetComponent<CallComponent>();
            var rid = callComponent.AddRequestCallBack(new SenderMessage(TimeHelper.Now(), tcs));
            //
            InnerRequest innerRequest = new InnerRequest { Opcode = RpcManager.Instance.GetRequestOpcode(request.GetType()), Content = request.ToBinary(), Sn = rid };
            other.Tell(innerRequest);
            //
            long beginTime = TimeHelper.Now();
            IResponse response = await tcs;
            long cost = TimeHelper.Now() - beginTime;
            //
            if (cost >= 100)
            {
                Node.Logger.Warning($"call cost time:{cost} too long");
            }
            //
            return response;
        }

        public void Send(IActorRef other, IRequest request)
        {
            InnerRequest innerRequest = new InnerRequest { Opcode = 1, Content = request.ToBinary(), Sn = 0 };
            other.Tell(innerRequest);
        }

        public void RunResponse(InnerResponse respone)
        {
            if (!requestCallbackDic.TryGetValue(respone.Sn, out var senderMessage))
            {
                Node.Logger.Warning($"inner message callback:{respone.Sn} Name:{respone.Opcode} Code:{respone.Code} not found; maybe time out");
                return;
            }
            requestCallbackDic.Remove(respone.Sn);
            if (respone.Code != Code.Ok)
            {
                senderMessage.Tcs.SetException(new CodeException(respone.Code, respone.Code.ToString()));
            }
            else
            {
                var retType = RpcManager.Instance.GetResponseOpcode(respone.Opcode);
                var ret = SerializerHelper.FromBinary(retType, respone.Content) as IResponse;
                senderMessage.Tcs.SetResult(ret);
            }
        }


        public Task Tick(long dt)
        {
            var now = TimeHelper.Now();
            while (true)
            {
                if (requestCallbackDic.Count == 0)
                {
                    break;
                }
                var first = requestCallbackDic.First();
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
