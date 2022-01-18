using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.CustomAttribute.GameLife
{
    //启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
    public delegate Task GameLoad();
    //开始 第一个tick开始前  @param:crossDay 是否跨天
    public delegate Task GameStart(bool crossDay);
    //停止前
    public delegate Task GamePreStop();
    //停止
    public delegate Task GameStop();
    //上线
    public delegate Task GameOnline(BaseActor player, long lastLogoutTime);
    //下线
    public delegate Task GameOffline(BaseActor player);
    //tick
    public delegate Task GameTick(long dt);

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GameServiceAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public GameServiceAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Load
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GameLoadAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public GameLoadAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Start
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GameStartAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public GameStartAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //PreStop
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GamePreStopAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public GamePreStopAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Stop
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GameStopAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public GameStopAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Online
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GameOnlineAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public GameOnlineAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Offline
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GameOfflineAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public GameOfflineAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Offline
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GameTickAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public GameTickAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }
}
