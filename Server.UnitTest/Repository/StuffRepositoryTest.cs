using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Server.DbModels;
using Server.Models;
using Server.Repository;
using Server.Repository.Interfaces;
using Server.Shared;

namespace Server.UnitTest.Repository
{
    public class StuffRepositoryTest
    {
        private static readonly UserModel UserModelTest = new UserModel()
        {
            Id = "11",
            Name = "GivenName FamilyName",
            GivenName = "GivenName",
            FamilyName = "FamilyName",
            Email = "Email"
        };

        private static readonly DatumModel DatumModelTest = new DatumModel
        {
            Id = "1",
            Label = "Label",
            Description = "Description",
            OtherInfo = "OtherInfo",
            User = UserModelTest
        };

        private readonly TUser _dbUser = new TUser
        {
            UsrId = "11",
            UsrName = "GivenName FamilyName",
            UsrGivenName = "GivenName",
            UsrFamilyName = "FamilyName",
            UsrEmail = "Email"
        };

        private readonly TStuff _dbStuff = new TStuff
        {
            StfId = "1",
            StfUserId = "11",
            StfLabel = "Label",
            StfDescription = "Description",
            StfOtherInfo = "OtherInfo",
            StfCreatedAt = DateTime.UtcNow.ToString("o")
        };

        private readonly SqliteConnection _connection;
        private readonly StuffDbContext _context;
        private readonly IStuffRepo _stuffRepo;

        public StuffRepositoryTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<StuffDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new StuffDbContext(options);
            _context.Database.EnsureCreated();
            var mockAuth = Mock.Of<IUserAuthRepo>(x => x.GetCurrentUserAsync(It.IsAny<string>()) == Task.FromResult(_dbUser));
            _stuffRepo = new StuffRepo(_context, mockAuth);
        }

        [Fact]
        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
        }

        // ***** ***** ***** LIST
        [Fact]
        public async Task StuffRepo_GetListAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.Add(_dbStuff);
            _context.SaveChanges();

            // Act
            var repoResult = await _stuffRepo.GetListAsync(1);

            // Assert
            var expected = DatumModelTest.Id;
            var actual = repoResult.DatumList.ToArray()[0].Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task StuffRepo_GetListAsync_ShouldReturn_PageOne()
        {
            // Arrange
            int requestedPage = 2;
            _context.Add(_dbUser);
            _context.Add(_dbStuff);
            _context.SaveChanges();

            // Act
            var repoResult = await _stuffRepo.GetListAsync(requestedPage);

            // Assert
            var expected = 1;
            var actual = repoResult.Page;
            Assert.Equal(expected, actual);
        }

        // ***** ***** ***** SEARCH
        [Fact]
        public async Task StuffRepo_SearchListAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.Add(_dbStuff);
            _context.SaveChanges();

            // Act
            var repoResult = await _stuffRepo.SearchListAsync("LABEL");

            // Assert
            var expected = DatumModelTest.Id;
            var actual = repoResult.DatumList.ToArray()[0].Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task StuffRepo_SearchListAsync_ShouldThrow_ArgumentException()
        {
            // Arrange
            _context.Add(_dbUser);
            var dbStuffList = new List<TStuff>();
            for (int idx = 0; idx < 7; idx++)
            {
                var tmpStuff = new TStuff
                {
                    StfId = _dbStuff.StfId + (idx + 1), StfUserId = _dbStuff.StfUserId, StfLabel = _dbStuff.StfLabel
                };
                dbStuffList.Add(tmpStuff);
            }

            _context.AddRange(dbStuffList);
            _context.SaveChanges();

            // Act
            var repoResult = _stuffRepo.SearchListAsync("LABEL");
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Too many results. Please narrow your search.", exception.Message);
        }

        // ***** ***** ***** CREATE
        [Fact]
        public async Task StuffRepo_CreateAsync_ShouldReturn_Ok()
        {
            // Arrange1
            // Existing user
            _context.Add(_dbUser);
            _context.SaveChanges();

            // Act1
            var repoResult = await _stuffRepo.CreateAsync(DatumModelTest);

            // Assert1
            int actual = repoResult.Id.Count(x => x == '-');
            int expected = 4;
            Assert.Equal(expected, actual);

            // ***
            // Arrange2
            // Creating user at the same time as stuff
            DatumModelTest.User = null;

            // Act2
            repoResult = await _stuffRepo.CreateAsync(DatumModelTest);

            // Assert2
            actual = repoResult.Id.Count(x => x == '-');
            expected = 4;
            Assert.Equal(expected, actual);

            // Restore
            DatumModelTest.User = UserModelTest;
        }

        [Fact]
        public async Task StuffRepo_CreateAsync_ShouldReturn_ArgumentException()
        {
            // Arrange
            DatumModelTest.Label = string.Empty;
            _context.Add(_dbUser);
            _context.SaveChanges();

            // Act
            var repoResult = _stuffRepo.CreateAsync(DatumModelTest);
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The label cannot be empty.", exception.Message);

            // Restore
            DatumModelTest.Label = "Label";
        }

        // ***** ***** ***** READ SINGLE
        [Fact]
        public async Task StuffRepo_ReadAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.Add(_dbStuff);
            _context.SaveChanges();

            // Act
            var repoResult = await _stuffRepo.ReadAsync("1");

            // Assert
            var expected = DatumModelTest.Id;
            var actual = repoResult.Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task StuffRepo_ReadAsync_ShouldThrow_NotFoundException()
        {
            // Arrange
            // No stuff

            // Act
            var repoResult = _stuffRepo.ReadAsync("2");
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal("Stuff not found.", exception.Message);
        }

        // ***** ***** ***** UPDATE
        [Fact]
        public async Task StuffRepo_UpdateAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.Add(_dbStuff);
            _context.SaveChanges();

            // Act
            var repoResult = await _stuffRepo.UpdateAsync("1", DatumModelTest);

            // Assert
            var expected = DatumModelTest.Id;
            var actual = repoResult.Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task StuffRepo_UpdateAsync_ShouldThrow_ArgumentException()
        {
            // Arrange1
            // _datumModelTest.Id != input.Id

            // Act1
            var repoResult = _stuffRepo.UpdateAsync("2", DatumModelTest);
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert1
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);

            // ***
            // Arrange2
            DatumModelTest.Id = "StuffNotFound";

            // Act2
            repoResult = _stuffRepo.UpdateAsync("StuffNotFound", DatumModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert2
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);
            // Restore
            DatumModelTest.Id = "1";

            // ***
            // Arrange3
            DatumModelTest.Label = string.Empty;

            // Act3
            repoResult = _stuffRepo.UpdateAsync(DatumModelTest.Id, DatumModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert3
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The label cannot be empty.", exception.Message);
            // Restore
            DatumModelTest.Label = "Label";
            
            // ***
            // Arrange4
            _dbUser.UsrId = "2";
            _dbStuff.StfUserId = "2";
            _context.Add(_dbUser);
            _context.Add(_dbStuff);
            _context.SaveChanges();
            _dbUser.UsrId = "11";

            // Act4
            repoResult = _stuffRepo.UpdateAsync("1", DatumModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert4
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);

            // Restore
            _dbStuff.StfUserId = "11";
            _dbStuff.StfUser.UsrId = "11";
        }

        // ***** ***** ***** DELETE
        [Fact]
        public async Task StuffRepo_DeleteAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.Add(_dbStuff);
            _context.SaveChanges();

            // Act
            await _stuffRepo.DeleteAsync("1");
            var actual = _context.TStuff.FirstOrDefault(x => x.StfId.Equals("1"));

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task StuffRepo_DeleteAsync_ShouldThrow_ArgumentException()
        {
            // Arrange1
            // No stuff

            // Act1
            var repoResult = _stuffRepo.DeleteAsync("2");
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert1
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);

            // ***
            // Arrange2
            _dbUser.UsrId = "2";
            _dbStuff.StfUserId = "2";
            _context.Add(_dbUser);
            _context.Add(_dbStuff);
            _context.SaveChanges();
            _dbUser.UsrId = "11";

            // Act2
            repoResult = _stuffRepo.DeleteAsync("1");
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert2
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);

            // Restore
            _dbStuff.StfUserId = "11";
            _dbStuff.StfUser.UsrId = "11";
        }
    }
}
