using Akka.Actor;
using Base;
using Base.Helper;
using Base.Network;
using Base.Serializer;
using Message;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Home.Model
{
    public class PlayerActor : BaseActor
    {
        IBaseSocketConnection session;
        public ulong PlayerId;
        private ILog _log;
        public override ILog Logger { get { if (_log == null) { _log = new NLogAdapter($"player:{PlayerId}"); } return _log; } }
        public readonly SortedDictionary<ulong, SenderMessage> InnerRequestCallback = new SortedDictionary<ulong, SenderMessage>();
        public readonly SortedDictionary<ulong, SenderMessage> OuterRequestCallback = new SortedDictionary<ulong, SenderMessage>();
        public IActorRef worldShardProxy;

        public PlayerActor() : base()
        {
            PlayerId = 0; //todo 从自己的地址中分析出来
            session = c;
            PlayerAttrManager.Instance.Hotfix.AddComponent(this);
        }   

        protected override async void PreStart()
        {   
            base.PreStart();
            await PlayerAttrManager.Instance.Hotfix.Load(this);
            await PlayerAttrManager.Instance.Hotfix.Start(this, false);
            EnterUpState();
        }


        protected override async void PostStop()
        {
            await PlayerAttrManager.Instance.Hotfix.PreStop(this);
            await PlayerAttrManager.Instance.Hotfix.Stop(this);
            base.PostStop();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case TickT tik:
                    {
                        var now = TimeHelper.Now();
                        Tick(now);
                        break;
                    }
                case ReceiveTimeout m:
                    {
                        ElegantStop();
                        break;
                    }

                case Request request:
                    {
                        //todo 如果sn为0 清空所有缓存消息
                        //todo 如果sn已存在直接回传sn
                        //todo 如果sn不存在且不等于lastSn+1 触发断线
                        //
                        RpcManager.Instance.OuterHandlerDispatcher.Dispatcher(this, request);
                        break;
                    }
                case InnerRequest request:
                    {
                        RpcManager.Instance.InnerHandlerDispatcher.Dispatcher(this, request);
                        break;
                    }
            }
        }

        async void Tick(long now)
        {
            await PlayerAttrManager.Instance.Hotfix.Tick(this, now);
        }

        public async Task Send(Response message)
        {
            await session.Send(message.ToBinary());
        }
    }
}
