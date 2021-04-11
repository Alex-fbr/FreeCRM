using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace WebAPI.FilterAttributes
{
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionHandlerAttribute> _logger;

        public ExceptionHandlerAttribute(ILogger<ExceptionHandlerAttribute> logger)
        {
            _logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, $"Error in {context.ActionDescriptor.DisplayName}.");

            context.Result = new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Content = context.Exception.Message,
                ContentType = "application/json"
            };

            base.OnException(context);
        }
    }
}
