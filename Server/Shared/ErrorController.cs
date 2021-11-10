using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Server.Shared
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;

        public ErrorController(IWebHostEnvironment env, ILogger<ErrorController> logger)
        {
            _env = env;
            _logger = logger;
        }

        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context == null)
            {
                return Ok();
            }

            var exception = context.Error;
            _logger.LogCritical(exception, "Error");
            var msg = exception.InnerException == null
                ? exception.Message
                : exception.InnerException.Message;

            if (exception is NotFoundException)
            {
                return new NotFoundObjectResult(new ErrorModel { Error = msg });
            }

            var error = new ErrorModel { Error = _env.IsDevelopment() ? msg : "An error occured. Please try again later." };
            return new BadRequestObjectResult(error);
        }
    }
}
