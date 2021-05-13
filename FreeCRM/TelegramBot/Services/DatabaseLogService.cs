using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DatabaseLogService(ILoggerFactory logger, ITelegramBaseDbContext dbContext, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger.CreateLogger<DatabaseLogService>() ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public async Task ParseUpdateAsync(Update update)
        {
            try
            {
                switch (update.Type)
                {
                    case Telegram.Bot.Types.Enums.UpdateType.Message:
                        await LogMessage(update.Message);
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.EditedMessage:
                        await LogMessage(update.EditedMessage);
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.ChannelPost:
                        await LogMessage(update.ChannelPost);
                        break;

                    case Telegram.Bot.Types.Enums.UpdateType.EditedChannelPost:
                        await LogMessage(update.EditedChannelPost);
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

        private async Task LogMessage(Message message)
        {
            _logger.LogDebug($"Message: {JsonSerializer.Serialize(message)}");

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = (TelegramBaseDbContext)scope.ServiceProvider.GetService<ITelegramBaseDbContext>();

            await DatabaseAction.TryDatabaseAction(AddOrUpdateUser(dbContext, message.From), _logger, "Ошибка при работе с пользователем");
            await DatabaseAction.TryDatabaseAction(AddOrUpdateChat(dbContext, message.Chat), _logger, "Ошибка при работе с чатом");
            await DatabaseAction.TryDatabaseAction(AddOrUpdateMessage(dbContext, message), _logger, "Ошибка при работе с чатом");
        }

        private async Task AddOrUpdateUser(TelegramBaseDbContext dbContext, User user)
        {
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

            if (dbUser == null)
            {
                dbContext.Users.Add(new DAL.Entities.User()
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
                dbContext.Users.Update(dbUser);
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task AddOrUpdateChat(TelegramBaseDbContext dbContext, Chat chat)
        {
            var dbChat = dbContext.Chats.FirstOrDefault(x => x.Id == chat.Id);
            var dbChatPermission = dbContext.ChatPermissions.FirstOrDefault(x => x.ChatId == chat.Id);

            if (dbChat == null)
            {
                dbContext.Chats.Add(new DAL.Entities.Chat()
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
                dbContext.Chats.Update(dbChat);
            }

            if (dbChatPermission == null)
            {
                dbContext.ChatPermissions.Add(new DAL.Entities.ChatPermission()
                {
                    ChatId = chat.Id,
                    CanAddWebPagePreviews = chat.Permissions?.CanAddWebPagePreviews,
                    CanChangeInfo = chat.Permissions?.CanChangeInfo,
                    CanInviteUsers = chat.Permissions?.CanInviteUsers,
                    CanPinMessages = chat.Permissions?.CanPinMessages,
                    CanSendMediaMessages = chat.Permissions?.CanSendMediaMessages,
                    CanSendMessages = chat.Permissions?.CanSendMessages,
                    CanSendOtherMessages = chat.Permissions?.CanSendOtherMessages,
                    CanSendPolls = chat.Permissions?.CanSendPolls,
                });
            }
            else
            {
                dbChatPermission.ChatId = chat.Id;
                dbChatPermission.CanAddWebPagePreviews = chat.Permissions?.CanAddWebPagePreviews;
                dbChatPermission.CanChangeInfo = chat.Permissions?.CanInviteUsers;
                dbChatPermission.CanInviteUsers = chat.Permissions?.CanInviteUsers;
                dbChatPermission.CanPinMessages = chat.Permissions?.CanPinMessages;
                dbChatPermission.CanSendMediaMessages = chat.Permissions?.CanSendMediaMessages;
                dbChatPermission.CanSendMessages = chat.Permissions?.CanSendMessages;
                dbChatPermission.CanSendOtherMessages = chat.Permissions?.CanSendOtherMessages;
                dbChatPermission.CanSendPolls = chat.Permissions?.CanSendPolls;
                dbContext.ChatPermissions.Update(dbChatPermission);
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task AddOrUpdateMessage(TelegramBaseDbContext dbContext, Message message)
        {
            var dbMessage = dbContext.Messages.FirstOrDefault(x => x.Id == message.MessageId);

            dbContext.Messages.Add(new DAL.Entities.Message()
            {
                Id = message.MessageId,
                AuthorSignature = message.AuthorSignature,
                Caption = message.Caption,
                ChannelChatCreated = message.ChannelChatCreated,
                ChatId = message.Chat.Id,
                ConnectedWebsite = message.ConnectedWebsite,
                Date = message.Date,
                DeleteChatPhoto = message.DeleteChatPhoto,
                EditDate = message.EditDate,
                ForwardDate = message.ForwardDate,
                ForwardFromMessageId = message.ForwardFromMessageId,
                ForwardSenderName = message.ForwardSenderName,
                ForwardSignature = message.ForwardSignature,
                FromUserId = message.From.Id,
                GroupChatCreated = message.GroupChatCreated,
                MediaGroupId = message.MediaGroupId,
                MigrateFromChatId = message.MigrateFromChatId,
                MigrateToChatId = message.MigrateToChatId,
                NewChatTitle = message.NewChatTitle,
                SupergroupChatCreated = message.SupergroupChatCreated,
                Text = message.Text,
                UserId = message.From.Id
            });

            await dbContext.SaveChangesAsync();
        }


    }
}
