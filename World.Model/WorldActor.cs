using Akka.Actor;
using Base;
using Base.Helper;
using Message;

namespace World.Model;

public class WorldActor : BaseActor
{
    public static readonly Props P = Props.Create<WorldActor>();
    private ActorLog? _log;


    public ulong WorldId { get; private set; }

    public override ActorLog Logger => _log ??= new ActorLog($"world:{WorldId}");


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
            case RequestWorld request:
            {
                RpcManager.Instance.InnerHandlerDispatcher?.Dispatcher(this, request);
                break;
            }
        }
    }
}