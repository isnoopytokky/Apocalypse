using System;

namespace Apocalypse.Logging.Concurrent.LoggingOperations
{
    sealed class LogWritingOperation : LoggingOperation
    {
        public LogWritingOperation(LogType type, string message, LogCategory category)
        {
            LogType = type;
            LogMessage = message;
            LogCategory = category;
        }

        public LogCategory LogCategory
        {
            get;
        }

        public string LogMessage
        {
            get;
        }

        public LogType LogType
        {
            get;
        }

        public override bool Execute(ILogger logger)
        {
            switch (LogType)
            {
                case LogType.Info:
                    logger.Info(LogMessage, LogCategory);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported log type: {LogType}.");
            }

            return true;
        }
    }
}
