namespace Apocalypse.Logging.Concurrent.LoggingOperations
{
    sealed class ShutdownOperation : LoggingOperation
    {
        public override bool Execute(ILogger logger)
        {
            return false;
        }
    }
}
