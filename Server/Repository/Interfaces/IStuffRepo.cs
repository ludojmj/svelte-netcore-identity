using System.Threading.Tasks;
using Server.Models;

namespace Server.Repository.Interfaces
{
    public interface IStuffRepo
    {
        Task<StuffModel> GetListAsync(int page);
        Task<StuffModel> SearchListAsync(string search);
        Task<DatumModel> CreateAsync(DatumModel input);
        Task<DatumModel> ReadAsync(string stuffId);
        Task<DatumModel> UpdateAsync(string stuffId, DatumModel input);
        Task DeleteAsync(string stuffId);
    }
}
