using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch;
using Base;
using Base.Helper;
using Base.Network;
using Base.Serialize;
using Common;
using Home.Model.Component;
using Message;
using Share.Model.Component;

namespace Home.Model;

public class PlayerActor : BaseActor
{
    public static readonly Props P = Props.Create<PlayerActor>();

    //链接id
    private string? _connectionId;

    private ActorLog? _log;

    //上次的登录key
    public string? LastLoginKey;

    public long _lastRequestTime = TimeHelper.NowSeconds();

    // path = akka://Z/system/sharding/Player/8714/4505283499219672065
    public PlayerActor()
    {
        uid = ulong.Parse(Self.Path.ToString().Split("/").Last());
        //有顺序
        AddComponent<CallComponent>();
        AddComponent<PlayerComponent>();
        AddComponent<BagComponent>();
        AddComponent<MailComponent>();
    }

    //获取channel
    private ICustomChannel? Channel =>
        Home.Instance.GetComponent<ConnectionComponent>().GetConnection(_connectionId);

    public ulong PlayerId => uid;

    private uint _lastPushSn { get; set; }
    private uint _nextPushSn => ++_lastPushSn;
    
    public override ActorLog Logger => _log ??= new ActorLog($"player:{PlayerId}");

    protected override void EnterUpState()
    {
        base.EnterUpState();
        _lastRequestTime = TimeHelper.NowSeconds();
    }

    protected override async void OnReceive(object message)
    {
        //消息处理
        switch (message)
        {
            case TickT:
            {
                var now = TimeHelper.Now();
                await Tick(now);
                CheckFree(now);
                break;
            }
            case ReceiveTimeout:
            {
                ElegantStop();
                break;
            }

            case Request request:
            {
                //
                try
                {
                    _lastRequestTime = TimeHelper.NowSeconds();
                    var disp = A.NotNull(RpcManager.Instance.OuterHandlerDispatcher, des: "outer dispatcher not found");
                    await disp.Dispatcher(this, request);
                }
                catch (CodeException e)
                {
                    //严重错误直接踢下线
                    // if (e.Serious)
                    // {
                    //     Channel?.Close();
                    //     _connectionId = null;
                    // }

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

    private void CheckFree(long now)
    {
        if (!LoadComplete) return;
        if (now - _lastRequestTime * 1000 <= GlobalParam.PLAYER_IDLE_RELEASE_TIME) return;
        ElegantStop();
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

    public async Task Push(IMessage msg)
    {
        var opcode = RpcManager.Instance.GetRequestOpcode(msg.GetType());
        await Send(msg, opcode, _nextPushSn);
    }

    private async Task KickOut()
    {
        var oldConn = Channel;
        if (oldConn != null)
        {
            await Push(new SLoginElsewhere());
            oldConn.Close();
            _connectionId = null;
        }
    }

    public async Task LoginPreDeal(C2SLogin request)
    {
        //清除老的链接
        await KickOut();
        //检查新链接
        var connect = Home.Instance.GetComponent<ConnectionComponent>()
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