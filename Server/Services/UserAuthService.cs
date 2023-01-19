using Microsoft.Extensions.Caching.Memory;
using Server.DbModels;
using Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Server.Services;

public class UserAuthService : IUserAuthService
{
    private const string CstCachePrefix = "AUTH_";
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILogger _logger;

    public UserAuthService(IMemoryCache cache, IHttpContextAccessor httpContext, ILogger<UserAuthService> logger)
    {
        _cache = cache;
        _httpContext = httpContext;
        _logger = logger;
    }

    public TUser GetCurrentUser(string operation)
    {
        if (_httpContext.HttpContext == null)
        {
            throw new ArgumentException("GetCurrentUser - HttpContext null");
        }

        string authToken = _httpContext.HttpContext.Request.Headers["Authorization"];
        string accessToken = authToken.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);
        string cacheKey = $"{CstCachePrefix}{accessToken}";
        if (_cache.TryGetValue(cacheKey, out TUser result))
        {
            _logger.LogInformation($"{result.UsrName} has {operation} at {DateTime.Now}");
            return result;
        }

        JwtSecurityToken decodeSub = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        IEnumerable<Claim> claimList = decodeSub.Claims.ToList();
        result = new TUser
        {
            UsrId = claimList.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value,
            UsrName = claimList.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value,
            UsrEmail = claimList.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50),
        };
        _cache.Set(cacheKey, result, cacheEntryOptions);

        _logger.LogInformation($"{result.UsrName} has {operation} at {DateTime.Now}");
        return result;
    }
}
