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

            textWriter.WriteLine($"LYRA - [{logEntry.LogLevel}] {message}");
        }
    }
}
