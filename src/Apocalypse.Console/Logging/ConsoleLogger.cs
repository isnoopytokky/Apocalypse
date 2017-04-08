using System;
using Apocalypse.Logging;

namespace Apocalypse.Console.Logging
{
    sealed class ConsoleLogger : ILogger
    {
        public void Dispose()
        {
        }

        public void Error(string message, LogCategory category)
        {
            WriteLog("Error", message, category);
        }

        public void Info(string message, LogCategory category = LogCategory.Apocalypse)
        {
            WriteLog("Info", message, category);
        }

        void WriteLog(string type, string message, LogCategory category)
        {
            var defaultColor = System.Console.ForegroundColor;

            try
            {
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.Write($"[{type}] ");
                System.Console.ForegroundColor = GetColorForCategory(category);
                System.Console.WriteLine(message);
            }
            finally
            {
                System.Console.ForegroundColor = defaultColor;
            }
        }

        static ConsoleColor GetColorForCategory(LogCategory category)
        {
            switch (category)
            {
                case LogCategory.Apocalypse:
                    return ConsoleColor.Cyan;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), "Unsupported category.");
            }
        }
    }
}
