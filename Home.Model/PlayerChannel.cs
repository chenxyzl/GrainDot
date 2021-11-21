using Akka.Actor;
using Base;
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
        public override void OnClose()
        {
            //通知actor下线
        }

        public override void OnConnected() { }

        public override void OnRecieve(byte[] bytes)
        {
            RpcMessage message;
            try
            {
                message = SerializerHelper.FromBinary<RpcMessage>(bytes);
            }
            catch (Exception e) //避免协议破解
            {
                logger.Warning(e.Message);
                Close();
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
                _ = Send(new RpcMessage { Code = e.Code }.ToBinary());
            }
            catch (Exception e) //不可预料的断开客户端链接
            {
                logger.Warning(e.Message);b`
                Close();
            }

        }

        public async Task BindPlayerActor(RpcMessage message)
        {
            await Base.Game.GameServer.GetChild("xx").Ask<int>(1, TimeSpan.FromSeconds(10));
        }

        public void TellSelf(RpcMessage message)
        {
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
            Base.Game.GameServer.GetChild("xx");
        }
    }

    static class PlayerChannelService
    {

    }
}
