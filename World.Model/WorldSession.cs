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
        public WorldActor World { get; private set; }
        public IActorRef Self { get; private set; }
        public ulong PlayerID { get; private set; }
        public WorldSession(WorldActor w, IActorRef s, ulong p)
        {
            World = w;
            Self = s;
            PlayerID = p;
        }

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
