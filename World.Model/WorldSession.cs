using Akka.Actor;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Model
{
    public class WorldSession
    {
        readonly public WorldActor World;
        readonly public IActorRef Self;
        readonly public ulong PlayerID;
        public void Send(IMessage message)
        {
            //创建sn
        }

        public void Replay(IMessage message)
        {
            //读取sn后返回
        }
    }
}
