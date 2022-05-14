using Common;

using Microsoft.EntityFrameworkCore;

using Serilog;

using Telegram.Bot;

using TelegramBot.DAL.Contexts;
using TelegramBot.Worker;
using TelegramBot.Worker.Configurations;
using TelegramBot.Worker.Services;
using TelegramBot.Worker.Services.Implementation;

IHost host = Host.CreateDefaultBuilder(args)
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

         services.AddSingleton<IUpdateHandlerService, UpdateHandlerService>();       
         services.AddSingleton<IRepositoryService, RepositoryService>();
         services.AddHostedService<Worker>();
     })
     .UseSerilog()
     .Build();

await host.RunAsync();



static void ConfigureTelegramDatabase(IServiceCollection services, IConfiguration configuration)
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
            throw new InvalidOperationException(TelegramBot.Worker.Properties.Resources.DatabaseTypeErrorMessage);
    }
}
