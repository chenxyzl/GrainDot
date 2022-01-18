using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class IServiceComponent : IActorComponent
    {
        public IServiceComponent(BaseActor a) : base(a) { }
        ////启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
        //public abstract Task Load();
        ////开始 第一个tick开始前  @param:crossDay 是否跨天
        //public abstract Task Start(bool crossDay);
        ////每一帧 @param:now 参数为服务器当前时间
        ////public abstract void Tick(long now);
        ////停止前
        //public abstract Task PreStop();
        ////停止
        //public abstract Task Stop();
        //跨0点 一般用于重置数据
        public abstract Task OnCrossDay();
        //有玩家上线
        public abstract Task PlayerOnline(UntypedActor actor, ulong playerId);
        //有玩家下线
        public abstract Task PlayerOffline(ulong playerId);
    }
}
