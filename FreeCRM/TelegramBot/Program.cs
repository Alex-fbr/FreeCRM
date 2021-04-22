using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace TelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ITelegramBotClient>(sc =>
                    {
                        var configuration = hostContext.Configuration;
                        var botConfig = new BotConfig();
                        configuration.GetSection(nameof(BotConfig)).Bind(botConfig);
                        return new TelegramBotClient(botConfig.Token);
                    });

                    services.AddHostedService<TelegramBotWorker>();
                });
    }
}
