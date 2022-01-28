using System;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.Helper;
using Base.Network;
using Base.Serialize;
using Home.Model.Component;
using Message;
using Share.Model.Component;

namespace Home.Model;

public class PlayerActor : BaseActor
{
    public static readonly Props P = Props.Create<PlayerActor>();
    private ILog _log;
    public ulong PlayerId;
    private IBaseSocketConnection session;

    public IActorRef worldShardProxy;

    public PlayerActor()
    {
        PlayerId = 0; //todo 从自己的地址中分析出来
        PlayerHotfixManager.Instance.Hotfix.AddComponent(this);
    }

    public override ILog Logger
    {
        get
        {
            if (_log == null) _log = new NLogAdapter($"player:{PlayerId}");

            return _log;
        }
    }

    protected override async void PreStart()
    {
        base.PreStart();
        await PlayerHotfixManager.Instance.Hotfix.Load(this);
        await PlayerHotfixManager.Instance.Hotfix.Start(this, false);
        EnterUpState();
    }


    protected override async void PostStop()
    {
        await PlayerHotfixManager.Instance.Hotfix.PreStop(this);
        await PlayerHotfixManager.Instance.Hotfix.Stop(this);
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
                //
                try
                {
                    RpcManager.Instance.OuterHandlerDispatcher.Dispatcher(this, request);
                }
                catch (CodeException e)
                {
                    //严重错误直接踢下线
                    if (e.Serious)
                    {
                        session.Close();
                        session = null;
                    }

                    Logger.Warning(e.ToString());
                }
                catch (Exception e)
                {
                    Logger.Warning(e.ToString());
                }

                break;
            }
            case InnerRequest request:
            {
                try
                {
                    RpcManager.Instance.InnerHandlerDispatcher.Dispatcher(this, request);
                }
                catch (Exception e)
                {
                    Logger.Warning(e.ToString());
                }

                break;
            }

            case InnerResponse response:
            {
                GetComponent<CallComponent>().RunResponse(response);
                break;
            }

            case SyncActorMessage msg:
            {
                break;
            }
        }
    }

    private async void Tick(long now)
    {
        await PlayerHotfixManager.Instance.Hotfix.Tick(this, now);
    }

    public async Task Send(Response message)
    {
        await session?.Send(message.ToBinary());
    }

    public void LoginPreDeal(Request request)
    {
        var c2sLogin = SerializeHelper.FromBinary<C2SLogin>(request.Content);
        var connectionId = c2sLogin.Unused;
        var connect = GameServer.Instance.GetHome().GetComponent<ConnectionDicCommponent>()
            .GetConnection(connectionId);
        session = connect;
    }
}

public static class ComponentExt
{
    public static PlayerActor Player(this IActorComponent self)
    {
        return self.Node as PlayerActor;
    }
}