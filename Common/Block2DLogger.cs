using NLog;

namespace Block2D.Common
{
    public abstract class Block2DLogger
    {
        private readonly string _loggerName;
        private readonly Logger _logger;

        public Block2DLogger(string loggerName)
        {
            _loggerName = loggerName;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void RiptideLog(string message)
        {
            _logger.Info(message);
        }

        public void LogInfo(string message)
        {
            _logger.Info("(" + _loggerName + "): " + message);
        }

        public void LogWarning(string message)
        {
            _logger.Warn("(" + _loggerName + "): " + message);

        }

        public void LogFatal(string message)
        {
            _logger.Fatal("(" + _loggerName + "): " + message);
        }
    }
}
