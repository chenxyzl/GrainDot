using Base.Helper;
using NLog.Config;

namespace Base;

public class NLogAdapter : ILog
{
    private readonly Logger logger;

    public NLogAdapter(string p)
    {
        LogManager.Configuration = new XmlLoggingConfiguration("../Conf/NLog.config");
        LogManager.Configuration.Variables["appIdFormat"] = $"{CommandHelper.Instance.NodeType}:{CommandHelper.Instance.NodeId}";
        logger = LogManager.GetLogger(p);
    }

    public void Trace(string message)
    {
        logger.Trace(message);
    }

    public void Warning(string message)
    {
        logger.Warn(message);
    }

    public void Info(string message)
    {
        logger.Info(message);
    }

    public void Debug(string message)
    {
        logger.Debug(message);
    }

    public void Error(string message)
    {
        logger.Error(message);
    }

    public void Fatal(string message)
    {
        logger.Fatal(message);
    }

    public void Trace(string message, params object[] args)
    {
        logger.Trace(message, args);
    }

    public void Warning(string message, params object[] args)
    {
        logger.Warn(message, args);
    }

    public void Info(string message, params object[] args)
    {
        logger.Info(message, args);
    }

    public void Debug(string message, params object[] args)
    {
        logger.Debug(message, args);
    }

    public void Error(string message, params object[] args)
    {
        logger.Error(message, args);
    }

    public void Fatal(string message, params object[] args)
    {
        logger.Fatal(message, args);
    }
}