namespace Message;

public interface IMessage
{
}

public interface IRequest : IMessage //知道actorRef直接发送的
{
}

public interface IResponse : IMessage //根据sender直接返回的
{
}

public interface IRequestPlayer : IRequest //从代理发送给玩家的
{
    ulong PlayerId { get; }
}

public interface IRequestWorld : IRequest //从代理发送给world
{
    ulong WorldId { get; }
}

public interface IHttpRequest : IRequest //http请求
{
}

public interface IHttpResponse : IResponse //http返回
{
}