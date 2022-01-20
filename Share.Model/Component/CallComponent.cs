using System;
using System.Collections.Generic;
using Base;
using Base.Helper;

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
    }
}
