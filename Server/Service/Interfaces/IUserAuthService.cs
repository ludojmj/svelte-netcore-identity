using System.Threading.Tasks;
using Server.DbModels;

namespace Server.Service.Interfaces
{
    public interface IUserAuthService
    {
        Task<TUser> GetCurrentUserAsync(string operation);
    }
}
