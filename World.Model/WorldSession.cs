using Akka.Actor;
using Message;

namespace World.Model;

public class WorldSession
{
    public WorldSession(WorldActor w, IActorRef s, ulong p)
    {
        World = w;
        Self = s;
        PlayerID = p;
    }

    public WorldActor World { get; }
    public IActorRef Self { get; }
    public ulong PlayerID { get; }

    public void Send(IMessage message)
    {
        //创建sn
    }

    public void Replay(IMessage message)
    {
        //读取sn后返回
    }
}