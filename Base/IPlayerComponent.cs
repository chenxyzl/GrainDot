using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public interface IPlayerHotfixLife
    {
        //添加组件
        public void AddComponent(BaseActor self);
        //启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
        public Task Load(BaseActor self);
        //开始 第一个tick开始前  @param:crossDay 是否跨天
        public Task Start(BaseActor self, bool crossDay);
        //停止前
        public Task PreStop(BaseActor self);
        //停止
        public Task Stop(BaseActor self);
        //每一帧 @param:now 参数为服务器当前时间
        public Task Online(BaseActor self, bool newLogin, long lastLogoutTime);
        //下线
        public Task Offline(BaseActor self);
        //tick
        public Task Tick(BaseActor self, long dt);
    }

    public abstract class IPlayerComponent : IActorComponent
    {
        public IPlayerComponent(BaseActor a) : base(a) { }
    }
}
