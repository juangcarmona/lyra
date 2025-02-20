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
        public static ServiceProvider ConfigureServices(bool convertToMp3, string destinationPath)
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
                .AddSingleton<DownloaderService>(sp =>
                    new DownloaderService(
                        sp.GetRequiredService<ILogger<DownloaderService>>(),
                        sp.GetRequiredService<ConversionService>(),
                        convertToMp3,
                        destinationPath))
                .AddSingleton<ConversionService>()
                .BuildServiceProvider();
        }
    }
}
