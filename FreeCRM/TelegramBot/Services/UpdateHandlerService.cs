using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Interfaces;
using static Common.CommandsSettingsXml;

namespace TelegramBot.Services
{
    public class UpdateHandlerService : IUpdateHandlerService
    {
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly CommandsSettingsXml _settings;

        public UpdateHandlerService(ILogger logger, ITelegramBotClient telegramBotClient)
        {
            _logger = logger;
            _botClient = telegramBotClient;

            var XMLFileName = $"{Environment.CurrentDirectory}\\settings.xml";

            if (System.IO.File.Exists(XMLFileName))
            {
                var ser = new XmlSerializer(typeof(CommandsSettingsXml));
                using var reader = new StreamReader(XMLFileName);
                _settings = ser.Deserialize(reader) as CommandsSettingsXml;
                reader.Close();
            }
        }

        public Task GetHandler(Update update) => update.Type switch
        {
            UpdateType.Message => BotOnMessageReceived(update.Message),
            UpdateType.EditedMessage => BotOnMessageReceived(update.Message),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery),

            UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery),
            UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult),
            UpdateType.ChannelPost => Task.Run(() => _logger.LogDebug("ChannelPost")),
            UpdateType.EditedChannelPost => Task.Run(() => _logger.LogDebug("EditedChannelPost")),
            UpdateType.ShippingQuery => Task.Run(() => _logger.LogDebug("ShippingQuery")),
            UpdateType.PreCheckoutQuery => Task.Run(() => _logger.LogDebug("PreCheckoutQuery")),
            UpdateType.Poll => Task.Run(() => _logger.LogDebug("Poll")),  // Poll - опрос 
            UpdateType.PollAnswer => Task.Run(() => _logger.LogDebug("PollAnswer")),
            _ => UnknownUpdateHandlerAsync(update)
        };

        private async Task BotOnMessageReceived(Message message)
        {
            _logger.LogDebug($"Receive message type: {message.Type}");

            if (message.Type != MessageType.Text)
            {
                await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: Properties.Resources.DoNotUnderstand, replyMarkup: new ReplyKeyboardRemove());
                await Usage(message);
                return;
            }

            var code = message.Text.Split(' ').First();
            var botCommand = _settings.BotCommandList.FirstOrDefault(x => x.Code == code);

            Task action;

            if (botCommand != null)
            {
                action = botCommand.Type switch
                {
                    CommandType.InlineKeyboard => SendInlineKeyboard(message, botCommand.KeyboardButtonList),
                    CommandType.ReplyKeyboard => SendReplyKeyboard(message, botCommand.KeyboardButtonList),
                    CommandType.GetPhoto => SendFile(message),
                    CommandType.Request => RequestContactAndLocation(message),
                    _ => Usage(message)
                };
            }
            else
            {
                action = Usage(message);
            }

            await action;
        }

        // Send inline keyboard
        async Task SendInlineKeyboard(Message message, List<List<Keyboard>> inlineKeyboards)
        {
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var inlineKeyboard = new List<List<InlineKeyboardButton>>();

            foreach (var (inlineKeyboardRow, list) in from inlineKeyboardRow in inlineKeyboards
                                                      let list = new List<InlineKeyboardButton>()
                                                      select (inlineKeyboardRow, list))
            {
                list.AddRange(inlineKeyboardRow.Select(ik => InlineKeyboardButton.WithCallbackData("\U00002714 " + ik.DisplayToUser, ik.CallbackData)));
                inlineKeyboard.Add(list);
            }

            var replyMarkup = new InlineKeyboardMarkup(inlineKeyboard);
            await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: Properties.Resources.Choose, replyMarkup: replyMarkup);
        }

        async Task SendReplyKeyboard(Message message, List<List<Keyboard>> keyboards)
        {
            var replyKeyboard = new List<List<KeyboardButton>>();

            foreach (var (inlineKeyboardRow, list) in from replyKeyboardRow in keyboards
                                                      let list = new List<KeyboardButton>()
                                                      select (replyKeyboardRow, list))
            {
                list.AddRange(inlineKeyboardRow.Select(ik => new KeyboardButton("\U0000274E " + ik.DisplayToUser)));
                replyKeyboard.Add(list);
            }

            var replyKeyboardMarkup = new ReplyKeyboardMarkup(replyKeyboard, resizeKeyboard: true);
            await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: Properties.Resources.Choose, replyMarkup: replyKeyboardMarkup);
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
                    KeyboardButton.WithRequestLocation("Отправить свою локацию"),
                    KeyboardButton.WithRequestContact("Отправить свой контакт"),
                });

            await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Оставьте свои данные", replyMarkup: RequestReplyKeyboard);
        }

        async Task Usage(Message message)
        {
            var usage = "Вот, что я могу:";

            foreach (var bc in _settings.BotCommandList)
            {
                usage = $"{usage} \n {bc.Code} - {bc.Description}";
                break;
            }

            await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: usage, replyMarkup: new ReplyKeyboardRemove());
        }

        // Process Inline Keyboard callback data
        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
            await _botClient.SendChatActionAsync(callbackQuery.Message.Chat.Id, ChatAction.Typing);

            //  await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Вы ответили '{callbackQuery.Data}'");

            var mes = callbackQuery.Message;
            mes.Text = callbackQuery.Data;
            await BotOnMessageReceived(mes);
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
