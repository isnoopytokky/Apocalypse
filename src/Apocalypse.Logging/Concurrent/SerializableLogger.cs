namespace Apocalypse.Logging.Concurrent
{
    public sealed class SerializableLogger : ILogger
    {
        readonly ILogger backed;

        public SerializableLogger(ILogger backed)
        {
            this.backed = backed;
        }

        public void Dispose()
        {
            lock (backed)
            {
                backed.Dispose();
            }
        }

        public void Error(string message, LogCategory category)
        {
            lock (backed)
            {
                backed.Error(message, category);
            }
        }

        public void Info(string message, LogCategory category)
        {
            lock (backed)
            {
                backed.Info(message, category);
            }
        }
    }
}
