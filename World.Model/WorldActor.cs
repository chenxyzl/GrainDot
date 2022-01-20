using System;
using Akka.Actor;
using Base;
using Base.Helper;
using Message;

namespace World.Model
{
    public class WorldActor : BaseActor
    {

        public ulong WorldId { get; private set; }
        private ILog _log;
        public override ILog Logger { get { if (_log == null) { _log = new NLogAdapter($"world:{WorldId}"); } return _log; } }
        public static readonly Props P = Props.Create<WorldActor>();


        public WorldActor() : base()
        {
            GameHotfixManager.Instance.Hotfix.AddComponent(this);
        }
        protected override async void PreStart()
        {
            base.PreStart();
            await GameHotfixManager.Instance.Hotfix.Load(this);
            await GameHotfixManager.Instance.Hotfix.Start(this, false);   
            EnterUpState();
        }


        protected override async void PostStop()
        {
            await GameHotfixManager.Instance.Hotfix.PreStop(this);
            await GameHotfixManager.Instance.Hotfix.Stop(this);
            base.PostStop();
        }
        async void Tick(long now)
        {
            await GameHotfixManager.Instance.Hotfix.Tick(this, now);
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
                case InnerRequest request:
                    {
                        RpcManager.Instance.InnerHandlerDispatcher.Dispatcher(this, request);
                        break;
                    }
            }
        }
    }
}
