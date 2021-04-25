using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Interfaces
{
    public interface IUpdateHandlerService
    {
        Task GetHandler(Update update);
    }
}
