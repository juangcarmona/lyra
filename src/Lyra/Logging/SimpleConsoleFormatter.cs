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

            // Set console color based on log level
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = logEntry.LogLevel switch
            {
                LogLevel.Information => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.DarkRed,
                _ => ConsoleColor.Gray
            };

            textWriter.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }
    }
}
