using System.Threading.Tasks;

namespace Base.CustomAttribute.GlobalLife;

//启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
public delegate Task GlobalLoad();

//开始 第一个tick开始前  @param:crossDay 是否跨天
public delegate Task GlobalStart();

//停止前
public delegate Task GlobalPreStop();

//停止
public delegate Task GlobalStop();

//tick
public delegate Task GlobalTick();

[AttributeUsage(AttributeTargets.Class)]
public class GlobalServiceAttribute : BaseAttribute
{
    public GlobalServiceAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Load
[AttributeUsage(AttributeTargets.Method)]
public class GlobalLoadAttribute : BaseAttribute
{
    public GlobalLoadAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Start
[AttributeUsage(AttributeTargets.Method)]
public class GlobalStartAttribute : BaseAttribute
{
    public GlobalStartAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//PreStop
[AttributeUsage(AttributeTargets.Method)]
public class GlobalPreStopAttribute : BaseAttribute
{
    public GlobalPreStopAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Stop
[AttributeUsage(AttributeTargets.Method)]
public class GlobalStopAttribute : BaseAttribute
{
    public GlobalStopAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Offline
[AttributeUsage(AttributeTargets.Method)]
public class GlobalTickAttribute : BaseAttribute
{
    public GlobalTickAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}