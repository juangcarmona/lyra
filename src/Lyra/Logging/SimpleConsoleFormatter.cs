using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Lyra.Logging
{
    public class SimpleConsoleFormatter : ConsoleFormatter
    {
        public SimpleConsoleFormatter() : base("simple") { }
        public override void Write<TState>(
            in LogEntry<TState> logEntry,
            IExternalScopeProvider? scopeProvider,
            TextWriter textWriter)
        {
            if (logEntry.Formatter == null)
                return;

            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (string.IsNullOrEmpty(message))
                return;

            // Use ANSI escape codes for color (PowerShell supports this)
            string colorCode = logEntry.LogLevel switch
            {
                LogLevel.Information => "\x1b[36m",  // Cyan
                LogLevel.Warning => "\x1b[33m",      // Yellow
                LogLevel.Error => "\x1b[31m",        // Red
                LogLevel.Critical => "\x1b[91m",     // Bright Red
                _ => "\x1b[37m"                      // White
            };

            string resetCode = "\x1b[0m"; // Reset color

            textWriter.WriteLine($"{colorCode}{message}{resetCode}");
        }
    }
}
