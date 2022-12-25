using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using IdentityModel.Client;
using Server.DbModels;
using Server.Services.Interfaces;

namespace Server.Services;

public class UserAuthService : IUserAuthService
{
    private const string CstCachePrefix = "AUTH_";
    private readonly IConfiguration _conf;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILogger _logger;

    public UserAuthService(IMemoryCache cache, IConfiguration conf, IHttpContextAccessor httpContext, ILogger<UserAuthService> logger)
    {
        _cache = cache;
        _conf = conf;
        _httpContext = httpContext;
        _logger = logger;
    }

    public async Task<TUser> GetCurrentUserAsync(string operation)
    {
        var accessToken = await _httpContext.HttpContext.GetTokenAsync("access_token");

        string cacheKey = $"{CstCachePrefix}{accessToken}";
        if (_cache.TryGetValue(cacheKey, out TUser result))
        {
            _logger.LogInformation($"{result.UsrName} has {operation} at {DateTime.Now}");
            return result;
        }

        IEnumerable<Claim> userClaimList;
        using (var httpClient = new HttpClient())
        {
            var userInfo = await httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = _conf["JwtToken:UserInfoService"],
                Token = accessToken
            });

            if (userInfo.IsError)
            {
                throw new ArgumentException(userInfo.Error);
            }

            userClaimList = userInfo.Claims;
        }

        JwtSecurityToken decodeSub = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var claimList = userClaimList.ToList();
        result = new TUser
        {
            UsrId = decodeSub?.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value,
            UsrName = claimList.FirstOrDefault(x => x.Type == "name")?.Value,
            UsrGivenName = claimList.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.GivenName)?.Value,
            UsrFamilyName = claimList.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.FamilyName)?.Value,
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