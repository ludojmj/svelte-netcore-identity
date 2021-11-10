using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.DbModels;
using Server.Models;
using Server.Repository.Interfaces;
using Server.Shared;

namespace Server.Repository
{
    public class StuffRepo : IStuffRepo
    {
        private const int CstItemsPerPage = 6;
        private readonly IUserAuthRepo _userAuth;
        private readonly StuffDbContext _context;

        public StuffRepo(StuffDbContext context, IUserAuthRepo userAuth)
        {
            _context = context;
            _userAuth = userAuth;
        }

        public async Task<StuffModel> GetListAsync(int page)
        {
            if (page == 0)
            {
                page = 1;
            }

            int dbCount = await _context.TStuff.CountAsync();
            int totalPages = ((dbCount - 1) / CstItemsPerPage) + 1;
            if (dbCount == 0 || page > totalPages)
            {
                page = 1;
            }

            ICollection<TStuff> dbStuffList = await _context.TStuff.AsQueryable()
                .OrderByDescending(x => x.StfUpdatedAt)
                .ThenByDescending(x => x.StfCreatedAt)
                .Skip(CstItemsPerPage * (page - 1))
                .Take(CstItemsPerPage)
                .Include(x => x.StfUser)
                .ToListAsync();
            var result = dbStuffList.ToStuffModel(page, dbCount, totalPages, CstItemsPerPage);
            return result;
        }

        public async Task<StuffModel> SearchListAsync(string search)
        {
            IQueryable<TStuff> query = _context.TStuff.Include(x => x.StfUser).Where(x =>
                EF.Functions.Like(x.StfLabel, $"%{search}%")
                || EF.Functions.Like(x.StfDescription, $"%{search}%")
                || EF.Functions.Like(x.StfOtherInfo, $"%{search}%")
                || EF.Functions.Like(x.StfUser.UsrGivenName, $"%{search}%")
                || EF.Functions.Like(x.StfUser.UsrFamilyName, $"%{search}%")
            );

            int dbCount = await query.CountAsync();
            if (dbCount > CstItemsPerPage)
            {
                throw new ArgumentException("Too many results. Please narrow your search.");
            }

            // Get stuff and their users.
            ICollection<TStuff> dbStuffList = await query.Include(x => x.StfUser).ToListAsync();
            var result = dbStuffList.ToStuffModel(1, dbCount, 1, CstItemsPerPage);
            return result;
        }

        public async Task<DatumModel> CreateAsync(DatumModel input)
        {
            input.CheckDatum();
            TStuff dbStuff = input.ToCreate();
            TUser dbUserAuth = await _userAuth.GetCurrentUserAsync("created datum");
            TUser dbUser = await _context.TUser.FirstOrDefaultAsync(x => x.UsrId.Equals(dbUserAuth.UsrId));
            if (dbUser == null)
            { // Create and attach new user
                dbUserAuth.UsrCreatedAt = DateTime.UtcNow.ToStrDate();
                dbStuff.StfUser = dbUserAuth;
            }

            // Attach foreign key
            dbStuff.StfUserId = dbUserAuth.UsrId;
            // Create stuff
            await _context.TStuff.AddAsync(dbStuff);
            await _context.SaveChangesAsync();
            // Attach user to the stuff for the response
            dbStuff.StfUser = dbUserAuth;
            var result = dbStuff.ToDatumModel();
            return result;
        }

        public async Task<DatumModel> ReadAsync(string stuffId)
        {
            // Get the stuff and its user
            TStuff dbStuff = await _context.TStuff
                .Where(x => x.StfId.Equals(stuffId))
                .Include(x => x.StfUser)
                .FirstOrDefaultAsync();

            if (dbStuff == null)
            {
                throw new NotFoundException("Stuff not found.");
            }

            var result = dbStuff.ToDatumModel();
            return result;
        }

        public async Task<DatumModel> UpdateAsync(string stuffId, DatumModel input)
        {
            input.CheckDatum();
            if (stuffId != input.Id)
            {
                throw new ArgumentException("Corrupted data.");
            }

            TUser dbUserAuth = await _userAuth.GetCurrentUserAsync("updated datum");
            TStuff dbStuff = await _context.TStuff.FirstOrDefaultAsync(x => x.StfId.Equals(stuffId));
            if (dbStuff == null || dbStuff.StfUserId != dbUserAuth.UsrId)
            {
                throw new ArgumentException("Corrupted data.");
            }

            // Update stuff
            dbStuff = input.ToUpdate(dbStuff);
            await _context.SaveChangesAsync();
            // Attach user to the stuff for the response
            dbStuff.StfUser = dbUserAuth;
            var result = dbStuff.ToDatumModel();
            return result;
        }

        public async Task DeleteAsync(string stuffId)
        {
            TUser dbUserAuth = await _userAuth.GetCurrentUserAsync("deleted datum");
            TStuff dbStuff = await _context.TStuff.FirstOrDefaultAsync(x => x.StfId.Equals(stuffId));
            if (dbStuff == null || dbStuff.StfUserId != dbUserAuth.UsrId)
            {
                throw new ArgumentException("Corrupted data.");
            }

            _context.TStuff.Remove(dbStuff);
            await _context.SaveChangesAsync();
        }
    }
}
