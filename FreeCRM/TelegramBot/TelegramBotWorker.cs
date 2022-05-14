using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

using TelegramBot.Worker.Interfaces;
using TelegramBot.Worker.Services;

namespace TelegramBot.Worker
{
    public class TelegramBotWorker : BackgroundService
    {
        private readonly ILogger<TelegramBotWorker> _logger;
        private readonly ITelegramBotClient _bot;
        private readonly IRepositoryService _databaseLog;
        private readonly QueuedUpdateReceiver _updateReceiver;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IUpdateHandlerService _updateHandlerService;

        public TelegramBotWorker(ILogger<TelegramBotWorker> logger, ITelegramBotClient telegramBotClient, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bot = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
            _updateReceiver = new QueuedUpdateReceiver(_bot);
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

            using var scope = _serviceScopeFactory.CreateScope();
            _databaseLog = scope.ServiceProvider.GetRequiredService<IRepositoryService>() ?? throw new ArgumentNullException(nameof(_databaseLog));
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("StartAsync");
            _updateHandlerService = new UpdateHandlerService(_logger, _bot);
            _updateReceiver.WithCancellation(cancellationToken: cancellationToken);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker stopping at: {time}", DateTimeOffset.Now);
            _updateReceiver.WithCancellation(cancellationToken);
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("ExecuteAsync");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await foreach (Update update in _updateReceiver.WithCancellation(stoppingToken))
                {
                    try
                    {
                        await _databaseLog.ParseUpdateAsync(update);
                        await _updateHandlerService.GetHandler(update);
                    }
                    catch (Exception exception)
                    {
                        HandleError(_bot, exception, stoppingToken);
                    }
                }
            }
        }

        private void HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogDebug(ErrorMessage);
        }

    }
}
