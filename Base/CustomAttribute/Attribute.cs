namespace Base;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class BaseAttribute : Attribute
{
    public BaseAttribute()
    {
        AttributeType = GetType();
    }

    public Type AttributeType { get; }
}