using System.Threading.Tasks;

namespace Base.CustomAttribute.PlayerLife;

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

[AttributeUsage(AttributeTargets.Class)]
public class PlayerServiceAttribute : BaseAttribute
{
    public PlayerServiceAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Load
[AttributeUsage(AttributeTargets.Method)]
public class PlayerLoadAttribute : BaseAttribute
{
    public PlayerLoadAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Start
[AttributeUsage(AttributeTargets.Method)]
public class PlayerStartAttribute : BaseAttribute
{
    public PlayerStartAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//PreStop
[AttributeUsage(AttributeTargets.Method)]
public class PlayerPreStopAttribute : BaseAttribute
{
    public PlayerPreStopAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Stop
[AttributeUsage(AttributeTargets.Method)]
public class PlayerStopAttribute : BaseAttribute
{
    public PlayerStopAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Online
[AttributeUsage(AttributeTargets.Method)]
public class PlayerOnlineAttribute : BaseAttribute
{
    public PlayerOnlineAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Offline
[AttributeUsage(AttributeTargets.Method)]
public class PlayerOfflineAttribute : BaseAttribute
{
    public PlayerOfflineAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Offline
[AttributeUsage(AttributeTargets.Method)]
public class PlayerTickAttribute : BaseAttribute
{
    public PlayerTickAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}