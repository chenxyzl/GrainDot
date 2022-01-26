using System.Threading.Tasks;
using Akka.Actor;

namespace Base.Game;

//启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
public delegate Task Load();

//开始 第一个tick开始前  @param:crossDay 是否跨天
public delegate Task Start(bool crossDay);

//停止前
public delegate Task PreStop();

//停止
public delegate Task Stop();

//每一帧 @param:now 参数为服务器当前时间
public delegate Task Online(IActorRef actor, ulong playerId);

//下线
public delegate Task Offline(ulong playerId);

//tick
public delegate Task Tick(long dt);