namespace Base;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class InnerRpcAttribute : BaseAttribute
{
}

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class GateRpcAttribute : BaseAttribute
{
}