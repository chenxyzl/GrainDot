namespace Base.State;

public class EmptyState : BaseState
{
    public override bool NeedSave { get; protected set; } = false;
}