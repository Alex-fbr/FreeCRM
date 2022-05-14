using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebAPI.FilterAttributes
{
    public class ProfilerAttribute : ActionFilterAttribute
    {
        private readonly ILogger<ProfilerAttribute> _logger;

        public ProfilerAttribute(ILogger<ProfilerAttribute> logger)
        {
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var timer = Stopwatch.StartNew();

            await next();

            timer.Stop();
            _logger.LogDebug($"The action '{context.ActionDescriptor.DisplayName}' took '{timer.Elapsed.TotalMilliseconds}' ms.");
        }
    }
}
