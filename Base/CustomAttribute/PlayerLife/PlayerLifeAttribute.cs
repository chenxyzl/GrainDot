using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.CustomAttribute.PlayerLife
{

    //启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
    public delegate Task PlayerLoad();
    //开始 第一个tick开始前  @param:crossDay 是否跨天
    public delegate Task PlayerStart(bool crossDay);
    //停止前
    public delegate Task PlayerPreStop();
    //停止
    public delegate Task PlayerStop();
    //每一帧 @param:now 参数为服务器当前时间
    public delegate Task PlayerOnline(bool newLogin, long lastLogoutTime);
    //下线
    public delegate Task PlayerOffline();
    //tick
    public delegate Task PlayerTick(long dt);

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PlayerServiceAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public PlayerServiceAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Load
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlayerLoadAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public PlayerLoadAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Start
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlayerStartAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public PlayerStartAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //PreStop
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlayerPreStopAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public PlayerPreStopAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Stop
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlayerStopAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public PlayerStopAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Online
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlayerOnlineAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public PlayerOnlineAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Offline
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlayerOfflineAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public PlayerOfflineAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }

    //Offline
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlayerTickAttribute : BaseAttribute
    {
        public Type AttributeType { get; }

        public PlayerTickAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }
}
