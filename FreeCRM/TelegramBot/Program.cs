using Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using System;

using Telegram.Bot;

using TelegramBot.DAL.Contexts;
using TelegramBot.Worker.Configurations;
using TelegramBot.Worker.Interfaces;
using TelegramBot.Worker.Services;

namespace TelegramBot.Worker
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
                    var configuration = hostContext.Configuration;
                    services.ConfigureLogger(configuration);
                    ConfigureTelegramDatabase(services, configuration);

                    services.AddSingleton<ITelegramBotClient>(sc =>
                    {
                        var botConfig = new BotConfig();
                        configuration.GetSection(nameof(BotConfig)).Bind(botConfig);
                        return new TelegramBotClient(botConfig.Token);
                    });

                    services.AddScoped<IRepositoryService, RepositoryService>();
                    services.AddHostedService<TelegramBotWorker>();
                })
                .UseSerilog();

        private static void ConfigureTelegramDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var dbConfig = new TelegramDatabaseConfiguration();
            configuration.GetSection(nameof(TelegramDatabaseConfiguration)).Bind(dbConfig);
            var connectionString = configuration.GetConnectionString(dbConfig.ConnectionStringName);

            switch (dbConfig.DatabaseType)
            {
                case DatabaseEnums.DatabaseTypes.Postgres:
                    services.AddDbContext<TelegramPostgresDbContext>(builder =>
                    {
                        builder.UseNpgsql(connectionString);
                    });

                    services.AddScoped<ITelegramBaseDbContext, TelegramPostgresDbContext>();
                    break;

                default:
                    throw new InvalidOperationException(Properties.Resources.DatabaseTypeErrorMessage);
            }
        }
    }
}
