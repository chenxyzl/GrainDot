namespace Base;

//Load
[AttributeUsage(AttributeTargets.Method)]
public class LoadAttribute : BaseAttribute
{
    public LoadAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Start
[AttributeUsage(AttributeTargets.Method)]
public class StartAttribute : BaseAttribute
{
    public StartAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//PreStop
[AttributeUsage(AttributeTargets.Method)]
public class PreStopAttribute : BaseAttribute
{
    public PreStopAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Stop
[AttributeUsage(AttributeTargets.Method)]
public class StopAttribute : BaseAttribute
{
    public StopAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}

//Tick
[AttributeUsage(AttributeTargets.Method)]
public class TickAttribute : BaseAttribute
{
    public TickAttribute()
    {
        AttrType = GetType();
    }

    public Type AttrType { get; }
}