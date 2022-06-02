using Server.DbModels;

namespace Server.Services.Interfaces;

public interface IUserAuthService
{
    Task<TUser> GetCurrentUserAsync(string operation);
}
