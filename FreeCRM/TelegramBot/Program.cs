using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;
using TelegramBot.Configurations;

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
                    var Configuration = hostContext.Configuration;
                    services.ConfigureLogger(Configuration);

                    services.AddSingleton<ITelegramBotClient>(sc =>
                    {
                        var botConfig = new BotConfig();
                        Configuration.GetSection(nameof(BotConfig)).Bind(botConfig);
                        return new TelegramBotClient(botConfig.Token);
                    });

                    services.AddHostedService<TelegramBotWorker>();
                })
                .UseSerilog();

    }
}
