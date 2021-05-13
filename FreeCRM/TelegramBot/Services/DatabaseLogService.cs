using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.DAL.Contexts;
using TelegramBot.DAL.Extensions;
using TelegramBot.Worker.Interfaces;

namespace TelegramBot.Worker.Services
{
    public class DatabaseLogService : IDatabaseLogService
    {
        private readonly ILogger _logger;
        private readonly TelegramBaseDbContext _dbContext;

        public DatabaseLogService(ILoggerFactory logger, ITelegramBaseDbContext dbContext)
        {
            _logger = logger.CreateLogger<DatabaseLogService>();
            _dbContext = (dbContext as TelegramBaseDbContext) ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task ParseUpdateAsync(Update update)
        {
            try
            {
                switch (update.Type)
                {
                    case Telegram.Bot.Types.Enums.UpdateType.Message:
                        LogMessage(update.Message);
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.EditedMessage:
                        LogMessage(update.EditedMessage);
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.ChannelPost:
                        LogMessage(update.ChannelPost);
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.EditedChannelPost:
                        LogMessage(update.EditedChannelPost);
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.InlineQuery:
                        _logger.LogWarning($"InlineQuery: {JsonSerializer.Serialize(update.InlineQuery)}");
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResult:
                        _logger.LogWarning($"ChosenInlineResult: {JsonSerializer.Serialize(update.ChosenInlineResult)}");
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                        _logger.LogWarning($"CallbackQuery: {JsonSerializer.Serialize(update.CallbackQuery)}");
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.ShippingQuery:
                        _logger.LogWarning($"ShippingQuery: {JsonSerializer.Serialize(update.ShippingQuery)}");
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.PreCheckoutQuery:
                        _logger.LogWarning($"PreCheckoutQuery: {JsonSerializer.Serialize(update.PreCheckoutQuery)}");
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.Poll:
                        _logger.LogWarning($"Poll: {JsonSerializer.Serialize(update.Poll)}");
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.PollAnswer:
                        _logger.LogWarning($"PollAnswer: {JsonSerializer.Serialize(update.PollAnswer)}");
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PollAnswer: {JsonSerializer.Serialize(update.PollAnswer)}");
            }

        }

        private void LogMessage(Message message)
        {
            _logger.LogDebug($"Message: {JsonSerializer.Serialize(message)}");

            DatabaseAction.TryDatabaseAction(() => AddOrUpdateUser(message.From), _logger, "Ошибка при работе с пользователем");
            DatabaseAction.TryDatabaseAction(() => AddOrUpdateChat(message.Chat), _logger, "Ошибка при работе с чатом");

        }

        private void AddOrUpdateChat(Chat chat)
        {
            var dbChat = _dbContext.Chats.FirstOrDefault(x => x.Id == chat.Id);

            if (dbChat == null)
            {
                _dbContext.Chats.Add(new DAL.Entities.Chat()
                {
                    Id = chat.Id,
                    Created = DateTime.UtcNow,
                    FirstName = chat.FirstName,
                    LastName = chat.LastName,
                    CanSetStickerSet = chat.CanSetStickerSet,
                    Description = chat.Description,
                    InviteLink = chat.InviteLink,
                    SlowModeDelay = chat.SlowModeDelay,
                    StickerSetName = chat.StickerSetName,
                    Title = chat.Title,
                    Username = chat.Username,
                    Type = (int)chat.Type
                });
            }
            else
            {
                dbChat.FirstName = chat.FirstName;
                dbChat.LastName = chat.LastName;
                dbChat.CanSetStickerSet = chat.CanSetStickerSet;
                dbChat.Description = chat.Description;
                dbChat.InviteLink = chat.InviteLink;
                dbChat.SlowModeDelay = chat.SlowModeDelay;
                dbChat.StickerSetName = chat.StickerSetName;
                dbChat.Title = chat.Title;
                dbChat.Username = chat.Username;
                dbChat.Type = (int)chat.Type;
                _dbContext.Chats.Update(dbChat);
            }

            _dbContext.SaveChanges();
        }

        private void AddOrUpdateUser(User user)
        {
            var dbUser = _dbContext.Users.FirstOrDefault(x => x.Id == user.Id);

            if (dbUser == null)
            {
                _dbContext.Users.Add(new DAL.Entities.User()
                {
                    Id = user.Id,
                    Created = DateTime.UtcNow,
                    CanJoinGroups = user.CanJoinGroups,
                    CanReadAllGroupMessages = user.CanReadAllGroupMessages,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.Username,
                    IsBot = user.IsBot,
                    LanguageCode = user.LanguageCode,
                    SupportsInlineQueries = user.SupportsInlineQueries
                });
            }
            else
            {
                dbUser.CanJoinGroups = user.CanJoinGroups;
                dbUser.CanReadAllGroupMessages = user.CanReadAllGroupMessages;
                dbUser.FirstName = user.FirstName;
                dbUser.LastName = user.LastName;
                dbUser.UserName = user.Username;
                dbUser.IsBot = user.IsBot;
                dbUser.LanguageCode = user.LanguageCode;
                dbUser.SupportsInlineQueries = user.SupportsInlineQueries;
                _dbContext.Users.Update(dbUser);
            }

            _dbContext.SaveChanges();
        }

    }
}
