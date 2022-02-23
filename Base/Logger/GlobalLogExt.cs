using Common;

namespace Base;

public class ActorLog
{
    private readonly string _tag;

    public ActorLog(string tag)
    {
        _tag = tag + "--:";
    }

    public void Trace(string message)
    {
        GlobalLog.Trace(_tag + message);
    }

    public void Warning(string message)
    {
        GlobalLog.Warning(_tag + message);
    }

    public void Info(string message)
    {
        GlobalLog.Info(_tag + message);
    }

    public void Debug(string message)
    {
        GlobalLog.Debug(_tag + message);
    }

    public void Error(Exception e)
    {
        GlobalLog.Error(_tag + e.ToString());
    }

    public void Error(string message)
    {
        GlobalLog.Error(_tag + message);
    }

    public void Fatal(Exception e)
    {
        GlobalLog.Fatal(_tag + e.ToString());
    }

    public void Fatal(string message)
    {
        GlobalLog.Fatal(_tag + message);
    }
}