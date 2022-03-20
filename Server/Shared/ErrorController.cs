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
        public IActionResult Error(
            [FromServices] IWebHostEnvironment env,
            [FromServices] ILogger<ErrorController> logger)
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context == null)
            {
                return Ok();
            }

            var exception = context.Error;
            logger.LogCritical(exception, "Error");
            var msg = exception.InnerException == null
                ? exception.Message
                : exception.InnerException.Message;

            if (exception is NotFoundException)
            {
                return NotFound(new ErrorModel { Error = msg });
            }

            var error = new ErrorModel { Error = env.IsDevelopment() ? msg : "An error occured. Please try again later." };
            return BadRequest(error);
        }
    }
}
