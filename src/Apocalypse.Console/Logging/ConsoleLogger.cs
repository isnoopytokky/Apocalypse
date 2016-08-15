using System;
using Apocalypse.Logging;

namespace Apocalypse.Console.Logging
{
    sealed class ConsoleLogger : ILogger
    {
        public void Dispose()
        {
        }

        public void Info(string message, LogCategory category = LogCategory.Apocalypse)
        {
            WriteLog("Info", message, category);
        }

        void WriteLog(string type, string message, LogCategory category)
        {
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.Write($"[{type}] ");

            switch (category)
            {
                case LogCategory.Apocalypse:
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), "Unsupported category.");
            }

            System.Console.WriteLine(message);
        }
    }
}
