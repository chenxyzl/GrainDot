using System.Threading.Tasks;

namespace Base.CustomAttribute.GameLife;

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

public delegate Task GameTick(long dt);

[AttributeUsage(AttributeTargets.Class)]
public class GameServiceAttribute : BaseAttribute
{
    public GameServiceAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Load
[AttributeUsage(AttributeTargets.Method)]
public class GameLoadAttribute : BaseAttribute
{
    public GameLoadAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Start
[AttributeUsage(AttributeTargets.Method)]
public class GameStartAttribute : BaseAttribute
{
    public GameStartAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//PreStop
[AttributeUsage(AttributeTargets.Method)]
public class GamePreStopAttribute : BaseAttribute
{
    public GamePreStopAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Stop
[AttributeUsage(AttributeTargets.Method)]
public class GameStopAttribute : BaseAttribute
{
    public GameStopAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Online
[AttributeUsage(AttributeTargets.Method)]
public class GameOnlineAttribute : BaseAttribute
{
    public GameOnlineAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Offline
[AttributeUsage(AttributeTargets.Method)]
public class GameOfflineAttribute : BaseAttribute
{
    public GameOfflineAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Offline
[AttributeUsage(AttributeTargets.Method)]
public class GameTickAttribute : BaseAttribute
{
    public GameTickAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}