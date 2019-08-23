using NLog;
using System;

namespace ProjectManagement.Logging
{
    public class LogHelper
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static void LogInfo(string message)
        {
            logger.Info(message);
        }

        public static void LogError(Exception exception)
        {
            logger.Error(exception);
        }

        public static void LogWarn(string message)
        {
            logger.Warn(message);
        }
    }
}
