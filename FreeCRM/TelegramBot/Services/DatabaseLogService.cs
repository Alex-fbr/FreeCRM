using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.DAL.Contexts;
using TelegramBot.DAL.Extensions;
using TelegramBot.Worker.Interfaces;

namespace TelegramBot.Worker.Services
{
    public class DatabaseLogService : IDatabaseLogService
    {
        private readonly ILogger _logger;
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
                    case UpdateType.Message:
                    case UpdateType.EditedMessage:
                    case UpdateType.ChannelPost:
                    case UpdateType.EditedChannelPost:
                        await LogMessage(update);
                        break;

                    case UpdateType.InlineQuery:
                        _logger.LogWarning($"InlineQuery: {JsonSerializer.Serialize(update.InlineQuery)}");
                        await LogInlineQuery(update);
                        break;

                    case UpdateType.ChosenInlineResult:
                        _logger.LogWarning($"ChosenInlineResult: {JsonSerializer.Serialize(update.ChosenInlineResult)}");
                        break;

                    case UpdateType.CallbackQuery:
                        _logger.LogWarning($"CallbackQuery: {JsonSerializer.Serialize(update.CallbackQuery)}");
                        await LogCallbackQueryQuery(update);
                        break;

                    case UpdateType.ShippingQuery:
                        _logger.LogWarning($"ShippingQuery: {JsonSerializer.Serialize(update.ShippingQuery)}");
                        break;

                    case UpdateType.PreCheckoutQuery:
                        _logger.LogWarning($"PreCheckoutQuery: {JsonSerializer.Serialize(update.PreCheckoutQuery)}");
                        break;

                    case UpdateType.Poll:
                        _logger.LogWarning($"Poll: {JsonSerializer.Serialize(update.Poll)}");
                        break;

                    case UpdateType.PollAnswer:
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

        private async Task LogCallbackQueryQuery(Update update)
        {
            _logger.LogDebug($"InlineQuery: {JsonSerializer.Serialize(update)}");
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = (TelegramBaseDbContext)scope.ServiceProvider.GetService<ITelegramBaseDbContext>();

            await DatabaseAction.TryDatabaseAction(AddCallbackQuery(dbContext, update, update.Id), _logger, "Ошибка при работе с сообщением");
            await dbContext.SaveChangesAsync();
        }

        private async Task LogInlineQuery(Update update)
        {
            _logger.LogDebug($"InlineQuery: {JsonSerializer.Serialize(update.InlineQuery)}");
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = (TelegramBaseDbContext)scope.ServiceProvider.GetService<ITelegramBaseDbContext>();

            await DatabaseAction.TryDatabaseAction(AddInlineQuery(dbContext, update, update.Id), _logger, "Ошибка при работе с сообщением");
            await dbContext.SaveChangesAsync();
        }

        private async Task LogMessage(Update update)
        {
            _logger.LogDebug($"Message: {JsonSerializer.Serialize(update)}");

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = (TelegramBaseDbContext)scope.ServiceProvider.GetService<ITelegramBaseDbContext>();

            if (update.Type == UpdateType.Message)
            {
                await DatabaseAction.TryDatabaseAction(AddOrUpdateUser(dbContext, update.Message.From), _logger, "Ошибка при работе с пользователем");
                await DatabaseAction.TryDatabaseAction(AddOrUpdateChat(dbContext, update.Message.Chat), _logger, "Ошибка при работе с чатом");
            }

            await DatabaseAction.TryDatabaseAction(AddMessage(dbContext, update.Message, update.Type), _logger, "Ошибка при работе с сообщением");
            await dbContext.SaveChangesAsync();
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
        }

        private async Task AddOrUpdateChat(TelegramBaseDbContext dbContext, Chat chat)
        {
            var dbChat = await dbContext.Chats.FirstOrDefaultAsync(x => x.Id == chat.Id);
            var dbChatPermission = await dbContext.ChatPermissions.FirstOrDefaultAsync(x => x.ChatId == chat.Id);

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
        }

        private async Task AddMessage(TelegramBaseDbContext dbContext, Message message, UpdateType type)
        {
            await dbContext.Messages.AddAsync(new DAL.Entities.Message()
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
                Type = (int)type,
                UserId = message.From.Id
            });
        }

        private async Task AddInlineQuery(TelegramBaseDbContext dbContext, Update update, int id)
        {
            //await dbContext.Messages.AddAsync(new DAL.Entities.Message()
            //{
            //    Id = id,
            //    AuthorSignature = update.InlineQuery.AuthorSignature,
            //    Caption = update.InlineQuery.Caption,
            //    ChannelChatCreated = update.InlineQuery.ChannelChatCreated,
            //    ChatId = update.InlineQuery.Chat.Id,
            //    ConnectedWebsite = update.InlineQuery.ConnectedWebsite,
            //    Date = update.InlineQuery.Date,
            //    DeleteChatPhoto = update.InlineQuery.DeleteChatPhoto,
            //    EditDate = update.InlineQuery.EditDate,
            //    ForwardDate = update.InlineQuery.ForwardDate,
            //    ForwardFromMessageId = update.InlineQuery.ForwardFromMessageId,
            //    ForwardSenderName = update.InlineQuery.ForwardSenderName,
            //    ForwardSignature = update.InlineQuery.ForwardSignature,
            //    FromUserId = update.InlineQuery.From.Id,
            //    GroupChatCreated = update.InlineQuery.GroupChatCreated,
            //    MediaGroupId = update.InlineQuery.MediaGroupId,
            //    MigrateFromChatId = update.InlineQuery.MigrateFromChatId,
            //    MigrateToChatId = update.InlineQuery.MigrateToChatId,
            //    NewChatTitle = update.InlineQuery.NewChatTitle,
            //    SupergroupChatCreated = update.InlineQuery.SupergroupChatCreated,
            //    Text = update.InlineQuery.Query,
            //    Type = (int)update.Type,
            //    UserId = update.InlineQuery.From.Id
            //});
        }

        private async Task AddCallbackQuery(TelegramBaseDbContext dbContext, Update update, int id)
        {
           // var dbMessage = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == callbackQuery.From.Id);

        }
    }
}
