using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch;
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

    //链接id
    private string? _connectionId;

    private ILog? _log;

    //上次的登录key
    public string? LastLoginKey;

    // path = akka://Z/system/sharding/Player/8714/4505283499219672065
    public PlayerActor()
    {
        uid = ulong.Parse(Self.Path.ToString().Split("/").Last());
        PlayerHotfixManager.Instance.Hotfix.AddComponent(this);
    }

    //获取channel
    private ICustomChannel? Channel
    {
        get
        {
            if (_connectionId == null) return null;

            return Home.Instance.GetComponent<ConnectionDicCommponent>().GetConnection(_connectionId);
        }
    }

    public ulong PlayerId => uid;

    private uint _lastPushSn { get; set; }
    private uint _nextPushSn => ++_lastPushSn;

    public override ILog Logger
    {
        get
        {
            if (_log == null) _log = new NLogAdapter($"player:{PlayerId}");
            return _log;
        }
    }

    protected override void PreStart()
    {
        ActorTaskScheduler.RunTask(
            async () =>
            {
                await PlayerHotfixManager.Instance.Hotfix.Load(this);
                await PlayerHotfixManager.Instance.Hotfix.Start(this, false);
                base.PreStart();
                EnterUpState();
            }
        );
    }


    protected override void PostStop()
    {
        ActorTaskScheduler.RunTask(
            async () =>
            {
                await PlayerHotfixManager.Instance.Hotfix.PreStop(this);
                await PlayerHotfixManager.Instance.Hotfix.Stop(this);
                base.PostStop();
            }
        );
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
                    RpcManager.Instance.OuterHandlerDispatcher?.Dispatcher(this, request);
                }
                catch (CodeException e)
                {
                    //严重错误直接踢下线
                    if (e.Serious)
                    {
                        Channel?.Close();
                        _connectionId = null;
                    }

                    Logger.Warning(e.ToString());
                }
                catch (Exception e)
                {
                    Logger.Warning(e.ToString());
                }

                break;
            }
            case RequestPlayer request:
            {
                try
                {
                    uid = request.PlayerId;
                    RpcManager.Instance.InnerHandlerDispatcher?.Dispatcher(this, request);
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

            //转线程
            case ResumeActor msg:
            {
                GetComponent<CallComponent>().ResumeActor(msg);
                break;
            }
        }
    }

    private async void Tick(long now)
    {
        await PlayerHotfixManager.Instance.Hotfix.Tick(this, now);
    }

    public async Task Send(IMessage? message, uint opcode, uint sn, Code code = Code.Ok)
    {
        //channel存在则发送，不存在就算了
        if (Channel != null)
        {
            var res = new Response {Sn = sn, Code = code, Opcode = opcode, Content = message?.ToBinary()};
            await Channel.Send(res.ToBinary(), res.Opcode);
        }
    }

    public async Task SendError(uint opcode, uint sn, Code code)
    {
        await Send(null, opcode, sn, code);
    }

    public async void Push(IMessage msg)
    {
        var opcode = RpcManager.Instance.GetRequestOpcode(msg.GetType());
        await Send(msg, opcode, _nextPushSn);
    }

    private void KickOut()
    {
        var oldConn = Channel;
        if (oldConn != null)
        {
            Push(new SLoginElsewhere());
            oldConn.Close();
            _connectionId = null;
        }
    }

    public void LoginPreDeal(C2SLogin request)
    {
        //清除老的链接
        KickOut();
        //检查新链接
        var connect = Home.Instance.GetComponent<ConnectionDicCommponent>()
            .GetConnection(request.Unused);
        A.NotNull(connect, Code.Error, "connect not found", true);
        _connectionId = request.Unused;
    }

    public void LoginAfterDeal(C2SLogin request)
    {
        if (request.IsReconnect)
        {
            //todo 断线重连则在这里处理
        }
        else
        {
            //每次重新登录重置pushId
            _lastPushSn = 0;
        }
    }
}