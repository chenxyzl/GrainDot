using Common;
using NLog.Config;

namespace Base;

public static class GlobalLog
{
    private static Logger? _globalLog;
    private static Logger globalLog => A.NotNull(_globalLog, des: "global log must init before use");
    private static readonly object _lockObj = new();


    public static void Init(RoleType r, ushort nodeId)
    {
        lock (_lockObj)
        {
            var tag = $"{r}:{nodeId}";
            if (_globalLog != null)
            {
                A.Abort(des: "logger init repeated");
            }

            LogManager.Configuration = new XmlLoggingConfiguration("../Conf/NLog.config");
            LogManager.Configuration.Variables["appIdFormat"] = tag;
            _globalLog = LogManager.GetLogger(tag);
        }
    }

    public static void Trace(string message)
    {
        globalLog.Trace(message);
    }

    public static void Warning(string message)
    {
        globalLog.Warn(message);
    }

    public static void Info(string message)
    {
        globalLog.Info(message);
    }

    public static void Debug(string message)
    {
        globalLog.Debug(message);
    }

    public static void Error(Exception e)
    {
        globalLog.Error(e.ToString());
    }

    public static void Error(string message)
    {
        globalLog.Error(message);
    }

    public static void Fatal(Exception e)
    {
        globalLog.Fatal(e.ToString());
    }

    public static void Fatal(string message)
    {
        globalLog.Fatal(message);
    }
}