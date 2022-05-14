using Telegram.Bot.Types;

namespace TelegramBot.Worker.Services
{
    public interface IUpdateHandlerService
    {
        Task GetHandler(Update update);
    }
}
