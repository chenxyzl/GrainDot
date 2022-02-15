using System.Collections.Generic;

namespace Base.ET;

public class ETCancellationToken
{
    private HashSet<Action>? actions = new();

    public void Add(Action callback)
    {
        // 如果action是null，绝对不能添加,要抛异常，说明有协程泄漏
        actions!.Add(callback);
    }

    public void Remove(Action callback)
    {
        actions?.Remove(callback);
    }

    public bool IsCancel()
    {
        return actions == null;
    }

    public void Cancel()
    {
        if (actions == null) return;

        Invoke();
    }

    private void Invoke()
    {
        var runActions = actions;
        actions = null;
        if (runActions != null)
            foreach (var action in runActions)
                action.Invoke();
    }
}