namespace Base;

[AttributeUsage(AttributeTargets.Class)]
public class HttpHandlerAttribute : BaseAttribute
{
    public readonly uint HttpId;

    public HttpHandlerAttribute(string router)
    {
        Router = router;
    }

    public string Router { get; }
}