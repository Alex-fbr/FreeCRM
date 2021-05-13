using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TelegramBot.DAL.Extensions
{
    public static class DatabaseAction
    {
        public static void TryDatabaseAction(Action action, [Required] ILogger logger, string exceptionMessage = "Error Database", [CallerMemberName] string methodName = "")
        {
            try
            {
                action.Invoke();
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
