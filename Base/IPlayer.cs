using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class IPlayer : IComponent
    {
        public IPlayer(BaseActor n) : base(n) { }
        //启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
        public abstract Task Load();
        //开始 第一个tick开始前  @param:crossDay 是否跨天
        public abstract Task Start();
        //每一帧 @param:now 参数为服务器当前时间
        //public abstract void Tick(long now);
        //停止前
        public abstract Task PreStop();
        //停止
        public abstract Task Stop();
        //跨0点 一般用于重置数据
        public abstract Task OnCrossDay(bool natural);

        public abstract Task Online(bool newLogin, long lastLogoutTime);

        public abstract Task Offline();
    }
}
