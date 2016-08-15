using System;

namespace Apocalypse.Logging
{
    public interface ILogger : IDisposable
    {
        void Info(string message, LogCategory category = LogCategory.Apocalypse);
    }
}
