using Akka.Actor;
using Base.Network;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model
{
    class PlayerChannel : TcpSocketConnection
    {
        IActorRef actor;
        public PlayerChannel(ITcpSocketServer server, IChannel channel, TcpSocketServerEvent<ITcpSocketServer, ITcpSocketConnection, byte[]> serverEvent) : base(server, channel, serverEvent) { }
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
}
