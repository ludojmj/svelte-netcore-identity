using System.Threading.Tasks;
using Server.DbModels;

namespace Server.Repository.Interfaces
{
    public interface IUserAuthRepo
    {
        Task<TUser> GetCurrentUserAsync(string operation);
    }
}
