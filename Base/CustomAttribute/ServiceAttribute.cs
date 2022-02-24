namespace Base;

public class ServiceAttribute : BaseAttribute
{
    public ServiceAttribute(Type type)
    {
        A.Ensure(typeof(IComponent).IsAssignableFrom(type), des: "type must inherit from IComponent");
        HostType = type;
    }

    public Type HostType { get; }
}