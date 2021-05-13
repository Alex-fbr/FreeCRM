using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TelegramBot.DAL.Extensions
{
    public static class DatabaseAction
    {
        public static async Task TryDatabaseAction(Task action, [Required] ILogger logger, string exceptionMessage = "Error Database", [CallerMemberName] string methodName = "")
        {
            try
            {
                await action;
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyExc)
            {
                logger.LogError(dbUpdateConcurrencyExc, $"[{methodName}]: '{exceptionMessage}'");
            }
            catch (DbUpdateException dbUpdateExc)
            {
                logger.LogError(dbUpdateExc, $"[{methodName}]: '{exceptionMessage}'");
            }
        }
    }
}
