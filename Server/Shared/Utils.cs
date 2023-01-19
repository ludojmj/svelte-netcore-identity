using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Server.Shared;

public static class Utils
{
    private const int CstMaxLength = 4096;

    public static (string clientId, string operation) GetClientAndOperation(this ActionContext context)
    {
        string operation = context.HttpContext.Request.Path.ToString();
        string authToken = context.HttpContext.Request.Headers["Authorization"];
        string clientId = authToken.ToLog();
        return (clientId, operation);
    }

    public static string Truncate(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= CstMaxLength ? value : value[..CstMaxLength];
    }

    private static string ToLog(this string authToken)
    {
        if (string.IsNullOrEmpty(authToken))
        {
            return "Anonymous";
        }

        string result;
        try
        {
            string accessToken = authToken.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);
            JwtSecurityToken decodeSub = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var claimList = decodeSub.Claims;
            result = claimList.FirstOrDefault(x => x.Type == "client_id")?.Value;
        }
        catch (Exception ex) when (ex is NullReferenceException || ex is ArgumentException)
        {
            result = "Anonymous";
        }

        return result;
    }
}
