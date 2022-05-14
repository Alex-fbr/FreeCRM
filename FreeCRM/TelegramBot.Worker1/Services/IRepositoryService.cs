using System.Threading.Tasks;

using Telegram.Bot.Types;

namespace TelegramBot.Worker.Services
{
    public interface IRepositoryService
    {
        Task ParseUpdateAsync(Update update);
    }
}
