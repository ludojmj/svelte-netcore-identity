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

namespace Server.UnitTest.Repository
{
    public class UserRepositoryTest
    {
        private static readonly UserModel UserModelTest = new UserModel()
        {
            Id = "11",
            Name = "GivenName FamilyName",
            GivenName = "GivenName",
            FamilyName = "FamilyName",
            Email = "Email",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        private readonly TUser _dbUser = new TUser
        {
            UsrId = "11",
            UsrName = "GivenName FamilyName",
            UsrGivenName = "GivenName",
            UsrFamilyName = "FamilyName",
            UsrEmail = "Email",
            UsrCreatedAt = DateTime.UtcNow.ToString("o"),
            UsrUpdatedAt = DateTime.UtcNow.ToString("o")
        };

        private readonly SqliteConnection _connection;
        private readonly StuffDbContext _context;
        private readonly IUserRepo _userRepo;

        public UserRepositoryTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<StuffDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new StuffDbContext(options);
            _context.Database.EnsureCreated();

            var mockAuth = Mock.Of<IUserAuthRepo>(x => x.GetCurrentUserAsync(It.IsAny<string>()) == Task.FromResult(_dbUser));
            _userRepo = new UserRepo(_context, mockAuth);
        }

        [Fact]
        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
        }

        // ***** ***** ***** LIST
        [Fact]
        public async Task UserRepo_GetListAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.SaveChanges();

            // Act
            var repoResult = await _userRepo.GetListAsync(1);

            // Assert
            var expected = UserModelTest.Id;
            var actual = repoResult.UserList.ToArray()[0].Id;
            Assert.Equal(expected, actual);
        }

        // ***** ***** ***** SEARCH
        [Fact]
        public async Task UserRepo_SearchListAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.SaveChanges();

            // Act
            var repoResult = await _userRepo.SearchListAsync("GIVENNAME");

            // Assert
            var expected = UserModelTest.Id;
            var actual = repoResult.UserList.ToArray()[0].Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UserRepo_SearchListAsync_ShouldThrow_ArgumentException()
        {
            // Arrange
            var dbUserList = new List<TUser>();
            for (int idx = 0; idx < 7; idx++)
            {
                var tpmUser = new TUser { UsrId = _dbUser.UsrId + idx, UsrGivenName = _dbUser.UsrGivenName + idx };
                dbUserList.Add(tpmUser);
            }

            _context.AddRange(dbUserList);
            _context.SaveChanges();

            // Act
            var repoResult = _userRepo.SearchListAsync("GIVENNAME");
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Too many results. Please narrow your search.", exception.Message);
        }

        // ***** ***** ***** CREATE
        [Fact]
        public async Task UserRepo_CreateAsync_ShouldReturn_Ok()
        {
            // Arrange
            // No user

            // Act
            var repoResult = await _userRepo.CreateAsync(UserModelTest);

            // Assert
            var expected = UserModelTest.Id;
            var actual = repoResult.Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UserRepo_CreateAsync_ShouldThrow_InvalidOperationException()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.SaveChanges();

            // Act
            var repoResult = _userRepo.CreateAsync(UserModelTest);
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
            // Id null
            Assert.Contains("The instance of entity type", exception.Message);
        }

        [Fact]
        public async Task UserRepo_CreateAsync_ShouldReturn_ArgumentException()
        {
            // Arrange1
            UserModelTest.Id = string.Empty;

            // Act1
            var repoResult = _userRepo.CreateAsync(UserModelTest);
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert1
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The id cannot be empty.", exception.Message);
            // Restore
            UserModelTest.Id = "11";

            // ***
            // Arrange2
            UserModelTest.GivenName = string.Empty;

            // Act2
            repoResult = _userRepo.CreateAsync(UserModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert2
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The given name cannot be empty.", exception.Message);
            // Restore
            UserModelTest.GivenName = "GivenName";

            // ***
            // Arrange3
            UserModelTest.FamilyName = string.Empty;

            // Act3
            repoResult = _userRepo.CreateAsync(UserModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert3
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The family name cannot be empty.", exception.Message);
            // Restore
            UserModelTest.FamilyName = "FamilyName";

            // ***
            // Arrange4
            UserModelTest.Email = string.Empty;

            // Act4
            repoResult = _userRepo.CreateAsync(UserModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert4
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The email cannot be empty.", exception.Message);
            // Restore
            UserModelTest.Email = "Email";
        }

        // ***** ***** ***** READ SINGLE
        [Fact]
        public async Task UserRepo_ReadAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.SaveChanges();

            // Act
            var repoResult = await _userRepo.ReadAsync("11");

            // Assert
            var expected = UserModelTest.Id;
            var actual = repoResult.Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UserRepo_ReadAsync_ShouldReturnNull()
        {
            // Arrange
            // No user

            // Act
            var repoResult = await _userRepo.ReadAsync("Unknown");

            // Assert
            UserModel expected = null;
            var actual = repoResult;
            Assert.Equal(expected, actual);
        }


        // ***** ***** ***** UPDATE
        [Fact]
        public async Task UserRepo_UpdateAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.SaveChanges();

            // Act
            var repoResult = await _userRepo.UpdateAsync("11", UserModelTest);

            // Assert
            var expected = UserModelTest.Id;
            var actual = repoResult.Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UserRepo_UpdateAsync_ShouldThrow_UserNotFound()
        {
            // Arrange
            // _userModelTest.Id = "Unknown";

            // Act
            var repoResult = _userRepo.UpdateAsync("11", UserModelTest);
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);
        }

        [Fact]
        public async Task UserRepo_UpdateAsync_ShouldThrow_ArgumentException()
        {
            // Arrange1
            // _userModelTest.Id != input.Id

            // Act1
            var repoResult = _userRepo.UpdateAsync("Fake", UserModelTest);
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert1
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);

            // ***
            // Arrange2
            UserModelTest.Id = string.Empty;

            // Act2
            repoResult = _userRepo.UpdateAsync(string.Empty, UserModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert2
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The id cannot be empty.", exception.Message);
            // Restore
            UserModelTest.Id = "11";

            // ***
            // Arrange3
            UserModelTest.GivenName = string.Empty;

            // Act3
            repoResult = _userRepo.UpdateAsync(UserModelTest.Id, UserModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert3
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The given name cannot be empty.", exception.Message);
            // Restore
            UserModelTest.GivenName = "GivenName";

            // ***
            // Arrange4
            UserModelTest.FamilyName = string.Empty;

            // Act4
            repoResult = _userRepo.UpdateAsync(UserModelTest.Id, UserModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert4
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The family name cannot be empty.", exception.Message);
            // Restore
            UserModelTest.FamilyName = "FamilyName";

            // ***
            // Arrange5
            UserModelTest.Email = string.Empty;

            // Act5
            repoResult = _userRepo.UpdateAsync(UserModelTest.Id, UserModelTest);
            exception = await Record.ExceptionAsync(() => repoResult);

            // Assert5
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("The email cannot be empty.", exception.Message);
            // Restore
            UserModelTest.Email = "Email";
        }

        // ***** ***** ***** DELETE
        [Fact]
        public async Task UserRepo_DeleteAsync_ShouldReturn_Ok()
        {
            // Arrange
            _context.Add(_dbUser);
            _context.SaveChanges();

            // Act
            await _userRepo.DeleteAsync("11");
            var actual = _context.TUser.FirstOrDefault(x => x.UsrId.Equals("1"));

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UserRepo_DeleteAsync_ShouldThrow_UserNotFound()
        {
            // Arrange
            // No user

            // Act
            var repoResult = _userRepo.DeleteAsync("11");
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);
        }

        [Fact]
        public async Task UserRepo_DeleteAsync_ShouldThrow_ArgumentException()
        {
            // Arrange
            // No user

            // Act
            var repoResult = _userRepo.DeleteAsync("Unknown");
            var exception = await Record.ExceptionAsync(() => repoResult);

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Corrupted data.", exception.Message);
        }
    }
}
