using Microsoft.EntityFrameworkCore;
using Server.DbModels;
using Server.Models;
using Server.Services.Interfaces;

namespace Server.Services;

public class UserService : IUserService
{
    private const int CstItemsPerPage = 6;
    private readonly IUserAuthService _userAuth;
    private readonly StuffDbContext _context;

    public UserService(StuffDbContext context, IUserAuthService userAuth)
    {
        _context = context;
        _userAuth = userAuth;
    }

    public async Task<DirectoryModel> GetListAsync(int page)
    {
        if (page == 0)
        {
            page = 1;
        }

        int dbCount = await _context.TUsers.CountAsync();
        int totalPages = ((dbCount - 1) / CstItemsPerPage) + 1;
        if (dbCount == 0 || page > totalPages)
        {
            page = 1;
            totalPages = 1;
        }

        ICollection<TUser> dbUserList = await _context.TUsers
            .OrderByDescending(x => x.UsrUpdatedAt)
            .ThenByDescending(x => x.UsrCreatedAt)
            .Skip(CstItemsPerPage * (page - 1))
            .Take(CstItemsPerPage)
            .ToListAsync();
        var result = dbUserList.ToDirectoryModel(page, dbCount, totalPages, CstItemsPerPage);
        return result;
    }

    public async Task<DirectoryModel> SearchListAsync(string search)
    {
        IQueryable<TUser> query = _context.TUsers.Where(x =>
            EF.Functions.Like(x.UsrGivenName, $"%{search}%")
            || EF.Functions.Like(x.UsrFamilyName, $"%{search}%")
        );

        int dbCount = await query.CountAsync();
        if (dbCount > CstItemsPerPage)
        {
            throw new ArgumentException("Too many results. Please narrow your search.");
        }

        List<TUser> dbUserList = await query.ToListAsync();
        var result = dbUserList.ToDirectoryModel(1, dbCount, 1, CstItemsPerPage);
        return result;
    }

    public async Task<UserModel> CreateAsync(UserModel input)
    {
        input.CheckUser();
        TUser dbUser = input.ToCreate();
        await _context.TUsers.AddAsync(dbUser);
        await _context.SaveChangesAsync();
        var result = dbUser.ToUserModel();
        return result;
    }

    public async Task<UserModel> ReadAsync(string userId)
    {
        TUser dbUser = await _context.TUsers.FirstOrDefaultAsync(x => x.UsrId == userId);
        var result = dbUser.ToUserModel();
        return result;
    }

    public async Task<UserModel> UpdateAsync(string userId, UserModel input)
    {
        input.CheckUser();

        if (userId != input.Id)
        {
            throw new ArgumentException("Corrupted data.");
        }

        TUser dbUserAuth = _userAuth.GetCurrentUser("updated user");
        TUser dbUser = await _context.TUsers.FirstOrDefaultAsync(x => x.UsrId == userId);
        if (dbUser == null || dbUser.UsrId != dbUserAuth.UsrId)
        {
            throw new ArgumentException("Corrupted data.");
        }

        dbUser = input.ToUpdate(dbUser);
        await _context.SaveChangesAsync();
        var result = dbUser.ToUserModel();
        return result;
    }

    public async Task DeleteAsync(string userId)
    {
        TUser dbUserAuth = _userAuth.GetCurrentUser("deleted user");
        TUser dbUser = await _context.TUsers.FirstOrDefaultAsync(x => x.UsrId == userId);
        if (dbUser == null || dbUser.UsrId != dbUserAuth.UsrId)
        {
            throw new ArgumentException("Corrupted data.");
        }

        _context.TUsers.Remove(dbUser);
        await _context.SaveChangesAsync();
    }
}
