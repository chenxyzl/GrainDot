using NLog;
using NLog.Config;
using System;

namespace Base
{
    public class NLogAdapter : ILog
    {
        private readonly Logger logger;

        public NLogAdapter(string p)
        {
#if DEBUG
            LogManager.Configuration = new XmlLoggingConfiguration("../../Boot/Nlog.config");
#else
            LogManager.Configuration = new XmlLoggingConfiguration("Nlog.config");
#endif
            LogManager.Configuration.Variables["appIdFormat"] = $"xxx";
            logger = LogManager.GetLogger(p);

        }

        public void Trace(string message)
        {
            this.logger.Trace(message);
        }

        public void Warning(string message)
        {
            this.logger.Warn(message);
        }

        public void Info(string message)
        {
            this.logger.Info(message);
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }

        public void Error(string message)
        {
            this.logger.Error(message);
        }

        public void Fatal(string message)
        {
            this.logger.Fatal(message);
        }

        public void Trace(string message, params object[] args)
        {
            this.logger.Trace(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            this.logger.Warn(message, args);
        }

        public void Info(string message, params object[] args)
        {
            this.logger.Info(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            this.logger.Debug(message, args);
        }

        public void Error(string message, params object[] args)
        {
            this.logger.Error(message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            this.logger.Fatal(message, args);
        }
    }
}