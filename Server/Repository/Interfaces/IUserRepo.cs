using System.Threading.Tasks;
using Server.Models;

namespace Server.Repository.Interfaces
{
    public interface IUserRepo
    {
        Task<DirectoryModel> GetListAsync(int page);
        Task<DirectoryModel> SearchListAsync(string search);
        Task<UserModel> CreateAsync(UserModel input);
        Task<UserModel> ReadAsync(string userId);
        Task<UserModel> UpdateAsync(string userId, UserModel input);
        Task DeleteAsync(string userId);
    }
}
