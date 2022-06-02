using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Shared;

[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public sealed class TraceHandlerFilterAttribute : ActionFilterAttribute
{
    private readonly ILogger _logger;

    public TraceHandlerFilterAttribute(ILogger<TraceHandlerFilterAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null)
        {
            base.OnActionExecuting(null);
            return;
        }

        string operation = context.HttpContext.Request.Path.ToString();
        foreach (var elt in context.ActionArguments)
        {
            var flux = JsonSerializer.Serialize(elt.Value, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation($"{operation}_Request : {flux}");
        }

        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context == null)
        {
            base.OnActionExecuted(null);
            return;
        }

        string operation = context.HttpContext.Request.Path.ToString();
        var result = context.Result;
        if (result is ObjectResult elt)
        {
            string flux = JsonSerializer.Serialize(elt.Value, new JsonSerializerOptions { WriteIndented = true }).Truncate();
            _logger.LogInformation($"{operation}_Response : {flux}");
        }

        base.OnActionExecuted(context);
    }
}

// Truncate trace
public static class StringExt
{
    private const int CstMaxLength = 2048;

    public static string Truncate(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= CstMaxLength ? value : value.Substring(0, CstMaxLength);
    }
}
