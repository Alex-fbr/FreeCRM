using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Worker.Interfaces
{
    public interface IDatabaseLogService
    {
        Task ParseUpdateAsync(Update update);
    }
}
