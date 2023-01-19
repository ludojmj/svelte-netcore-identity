using Server.DbModels;

namespace Server.Services.Interfaces;

public interface IUserAuthService
{
    TUser GetCurrentUser(string operation);
}
