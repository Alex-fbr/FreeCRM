using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;

using TelegramBot.Worker.Services;

namespace TelegramBot.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITelegramBotClient _bot;
        private readonly IRepositoryService _databaseLog;
        private readonly IUpdateHandlerService _updateHandlerService;
        private readonly QueuedUpdateReceiver _updateReceiver;

        public Worker(
            ILogger<Worker> logger,
            ITelegramBotClient telegramBotClient,
            IRepositoryService repositoryService,
            IUpdateHandlerService updateHandlerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bot = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
            _databaseLog = repositoryService ?? throw new ArgumentNullException(nameof(repositoryService));
            _updateHandlerService = updateHandlerService ?? throw new ArgumentNullException(nameof(updateHandlerService));
            _updateReceiver = new QueuedUpdateReceiver(_bot);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await foreach (var update in _updateReceiver.WithCancellation(stoppingToken))
                {
                    try
                    {
                        await _databaseLog.ParseUpdateAsync(update);
                        await _updateHandlerService.GetHandler(update);
                    }
                    catch (Exception exception)
                    {
                        HandleError(exception);
                    }
                }
            }
        }

        private void HandleError(Exception exception)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogDebug(errorMessage);
        }
    }
}