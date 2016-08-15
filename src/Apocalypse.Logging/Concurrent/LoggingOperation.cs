namespace Apocalypse.Logging.Concurrent
{
    abstract class LoggingOperation
    {
        protected LoggingOperation()
        {
        }

        public abstract bool Execute(ILogger logger);
    }
}
