using System;

namespace Apocalypse.Logging
{
    public interface ILogger : IDisposable
    {
        void Error(string message, LogCategory category);
        void Info(string message, LogCategory category);
    }
}
