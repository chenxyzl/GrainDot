using Akka.Actor;
using Base;
using Base.Helper;
using Base.Network;
using Base.Serializer;
using DotNetty.Transport.Channels;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model
{
    public class PlayerChannel : TcpSocketConnection
    {
        IActorRef actor;
        ILog logger;
        public PlayerChannel(ITcpSocketServer server, IChannel channel, TcpSocketServerEvent<ITcpSocketServer, ITcpSocketConnection, byte[]> serverEvent) : base(server, channel, serverEvent)
        {
            logger = new NLogAdapter(this.ConnectionId);
        }

        static Props Props = Props.Create(() => new PlayerActor();

        public override void OnClose()
        {
            //通知actor下线
            actor = null;
        }


        public override void OnConnected() { }

        public override async void OnRecieve(byte[] bytes)
        {
            Request message;
            try
            {
                message = SerializerHelper.FromBinary<Request>(bytes);
            }
            catch (Exception e) //避免协议破解
            {
                logger.Warning(e.Message);
                Close();
                return;
            }

            Response ret = new Response { Opcode = message.Opcode, Sn = message.Sn };

            //如果是ping直接回复pong
            if (message.Opcode == 200000)
            {
                ret.Code = Code.Ok;
                ret.Content = new S2CPong { Time = TimeHelper.Now() }.ToBinary();
                await Send(ret.ToBinary());
                return;
            }

            try
            {
                if (actor == null)
                {
                    _ = BindPlayerActor(message);
                }
                else
                {
                    TellSelf(message);
                }
            }
            catch (CodeException e) //可预料的返回客户端错误码
            {
                logger.Warning(e.Message);
                if (e.Serious)
                {
                    Close();
                }
                else
                {
                    ret.Code = e.Code;
                    _ = Send(ret.ToBinary());
                }

            }
            catch (Exception e) //不可预料的断开客户端链接
            {
                logger.Warning(e.Message);
                Close();
            }

        }

        public async void BindPlayerActor(Request message)
        {
            //第一条消息必须是登录
            A.Ensure(message.Opcode == 200003, Code.Error, "first message must login", true);
            A.Ensure(actor == null, Code.Error, "player has bind", true);
            actor = Boot.GameServer.GetChild("xx");
            //玩家没有获取到则断开链接让客户用重新走http登陆
            A.RequireNotNull(actor, Code.Error, "player actor not found, login api may be overdue， please relogin", true);
            //填充链接id
            var login = SerializerHelper.FromBinary<C2SLogin>(message.Content);
            login.Unused = this.ConnectionId;
            message.Content = login.ToBinary();
            //为了高性能 只有登录消息 走Ask 其他消息都走Tell (因为需要超时)
            var a = await actor.Ask<Request>(1, TimeSpan.FromSeconds(3));
            actor.Tell(message);
        }

        public void TellSelf(Request message)
        {
            A.RequireNotNull(actor, Code.Error, "must bind first", true);
            actor.Tell(message);
        }
    }

    class WsPlayerChannel : WebSocketConnection
    {
        IActorRef actor;
        public WsPlayerChannel(IWebSocketServer server, IChannel channel, TcpSocketServerEvent<IWebSocketServer, IWebSocketConnection, byte[]> serverEvent) : base(server, channel, serverEvent) { }
        public override void OnClose()
        {
            //通知actor下线
        }

        public override void OnConnected() { }

        public override void OnRecieve(byte[] bytes)
        {

            //第一个消息一定是bind actor
            if (actor == null)
            {
                BindPlayerActor();
                return;
            }
            //解析其他消息 tell给actor
            throw new NotImplementedException();
        }

        public void BindPlayerActor()
        {
            Base.Boot.GameServer.GetChild("xx");
        }
    }

    static class PlayerChannelService
    {

    }
}
