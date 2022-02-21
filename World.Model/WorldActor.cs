using Akka.Actor;
using Base;
using Base.Helper;
using Message;

namespace World.Model;

public class WorldActor : BaseActor
{
    public static readonly Props P = Props.Create<WorldActor>();
    private ILog? _log;


    public WorldActor()
    {
        //todo 这里添加component
    }

    public ulong WorldId { get; private set; }

    public override ILog Logger => _log ??= new NLogAdapter($"world:{WorldId}");


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