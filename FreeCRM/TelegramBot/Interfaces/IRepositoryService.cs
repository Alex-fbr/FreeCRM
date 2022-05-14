using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Worker.Interfaces
{
    public interface IRepositoryService
    {
        Task ParseUpdateAsync(Update update);
    }
}
