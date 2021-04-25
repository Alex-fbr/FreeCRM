using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Interfaces;

namespace TelegramBot.Services
{
    public class UpdateHandlerService : IUpdateHandlerService
    {
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _botClient;

        public UpdateHandlerService(ILogger logger, ITelegramBotClient telegramBotClient)
        {
            _logger = logger;
            _botClient = telegramBotClient;
        }

        public Task GetHandler(Update update) => update.Type switch
        {
            UpdateType.Message => BotOnMessageReceived(update.Message),
            UpdateType.EditedMessage => BotOnMessageReceived(update.Message),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery),
            UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery),
            UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult),
            UpdateType.ChannelPost => Task.Run(() => _logger.LogDebug("ChannelPost")),
            UpdateType.EditedChannelPost => Task.Run(() => _logger.LogDebug("ChannelPost")),
            UpdateType.ShippingQuery => Task.Run(() => _logger.LogDebug("ChannelPost")),
            UpdateType.PreCheckoutQuery => Task.Run(() => _logger.LogDebug("ChannelPost")),
            UpdateType.Poll => Task.Run(() => _logger.LogDebug("ChannelPost")),
            UpdateType.PollAnswer => Task.Run(() => _logger.LogDebug("ChannelPost")),
            _ => UnknownUpdateHandlerAsync(update)
        };

        private async Task BotOnMessageReceived(Message message)
        {
            _logger.LogDebug($"Receive message type: {message.Type}");

            if (message.Type != MessageType.Text)
                return;

            var action = (message.Text.Split(' ').First()) switch
            {
                "/inline" => SendInlineKeyboard(message),
                "/keyboard" => SendReplyKeyboard(message),
                "/developerPhoto" => SendFile(message),
                "/request" => RequestContactAndLocation(message),
                _ => Usage(message)
            };

            await action;

            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            async Task SendInlineKeyboard(Message message)
            {
                await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    }
                });

                await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Choose", replyMarkup: inlineKeyboard);
            }

            async Task SendReplyKeyboard(Message message)
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                    },
                    resizeKeyboard: true
                );

                await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Choose", replyMarkup: replyKeyboardMarkup);
            }

            async Task SendFile(Message message)
            {
                await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"Developer.jpg";
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                await _botClient.SendPhotoAsync(chatId: message.Chat.Id, photo: new InputOnlineFile(fileStream, fileName), caption: "My developer - Alex Skolnik");
            }

            async Task RequestContactAndLocation(Message message)
            {
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });

                await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Who or Where are you?", replyMarkup: RequestReplyKeyboard);
            }

            async Task Usage(Message message)
            {
                const string usage = "Вот, что я могу:\n" +
                                        "/inline   - send inline keyboard\n" +
                                        "/keyboard - send custom keyboard\n" +
                                        "/developerPhoto    - send a photo\n" +
                                        "/request  - request location or contact";

                await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: usage, replyMarkup: new ReplyKeyboardRemove());
            }
        }

        // Process Inline Keyboard callback data
        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
            await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Вы ответили '{callbackQuery.Data}'");
        }

        #region Inline Mode

        private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
        {
            _logger.LogDebug($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResultBase[] results =
            {
                new InlineQueryResultArticle(id: "3", title: "TgBots", inputMessageContent: new InputTextMessageContent("hello"))
            };

            await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, results, isPersonal: true, cacheTime: 0);
        }

        private async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
        {
            _logger.LogDebug($"Received inline result: {chosenInlineResult.ResultId}");
        }

        private async Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogDebug($"Unknown update type: {update.Type}");
        }



        #endregion
    }
}
