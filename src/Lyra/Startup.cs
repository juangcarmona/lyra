using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lyra.Services;
using Lyra.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Lyra
{
    public static class Startup
    {
        public static ServiceProvider ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            return new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders(); // Remove default providers
                    loggingBuilder.AddConsoleFormatter<SimpleConsoleFormatter, ConsoleFormatterOptions>();
                    loggingBuilder.AddConsole(options =>
                    {
                        options.FormatterName = "simple"; // Use custom formatter
                    });
                    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                })
                .AddSingleton<DownloaderService>()
                .AddSingleton<ConversionService>()
                .BuildServiceProvider();
        }
    }
}
