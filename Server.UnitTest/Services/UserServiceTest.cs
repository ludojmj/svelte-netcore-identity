using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Server.DbModels;
using Server.Models;
using Server.Services;
using Server.Services.Interfaces;

namespace Server.UnitTest.Service;

public class UserServiceTest
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
    private readonly IUserService _userService;

    public UserServiceTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<StuffDbContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new StuffDbContext(options);
        _context.Database.EnsureCreated();

        var mockAuth = Mock.Of<IUserAuthService>(x => x.GetCurrentUserAsync(It.IsAny<string>()) == Task.FromResult(_dbUser));
        _userService = new UserService(_context, mockAuth);
    }

    [Fact]
    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }

    // ***** ***** ***** LIST
    [Fact]
    public async Task UserService_GetListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.SaveChanges();

        // Act
        var serviceResult = await _userService.GetListAsync(1);

        // Assert
        var expected = UserModelTest.Id;
        var actual = serviceResult.UserList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** SEARCH
    [Fact]
    public async Task UserService_SearchListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.SaveChanges();

        // Act
        var serviceResult = await _userService.SearchListAsync("GIVENNAME");

        // Assert
        var expected = UserModelTest.Id;
        var actual = serviceResult.UserList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserService_SearchListAsync_ShouldThrow_ArgumentException()
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
        var serviceResult = _userService.SearchListAsync("GIVENNAME");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Too many results. Please narrow your search.", exception.Message);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task UserService_CreateAsync_ShouldReturn_Ok()
    {
        // Arrange
        // No user

        // Act
        var serviceResult = await _userService.CreateAsync(UserModelTest);

        // Assert
        var expected = UserModelTest.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserService_CreateAsync_ShouldThrow_InvalidOperationException()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.SaveChanges();

        // Act
        var serviceResult = _userService.CreateAsync(UserModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        // Id null
        Assert.Contains("The instance of entity type", exception.Message);
    }

    [Fact]
    public async Task UserService_CreateAsync_ShouldReturn_ArgumentException()
    {
        // Arrange1
        UserModelTest.Id = string.Empty;

        // Act1
        var serviceResult = _userService.CreateAsync(UserModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

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
        serviceResult = _userService.CreateAsync(UserModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

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
        serviceResult = _userService.CreateAsync(UserModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

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
        serviceResult = _userService.CreateAsync(UserModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert4
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The email cannot be empty.", exception.Message);
        // Restore
        UserModelTest.Email = "Email";
    }

    // ***** ***** ***** READ SINGLE
    [Fact]
    public async Task UserService_ReadAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.SaveChanges();

        // Act
        var serviceResult = await _userService.ReadAsync("11");

        // Assert
        var expected = UserModelTest.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserService_ReadAsync_ShouldReturnNull()
    {
        // Arrange
        // No user

        // Act
        var serviceResult = await _userService.ReadAsync("Unknown");

        // Assert
        var actual = serviceResult;
        Assert.Null(actual);
    }


    // ***** ***** ***** UPDATE
    [Fact]
    public async Task UserService_UpdateAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.SaveChanges();

        // Act
        var serviceResult = await _userService.UpdateAsync("11", UserModelTest);

        // Assert
        var expected = UserModelTest.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserService_UpdateAsync_ShouldThrow_UserNotFound()
    {
        // Arrange
        // _userModelTest.Id = "Unknown";

        // Act
        var serviceResult = _userService.UpdateAsync("11", UserModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);
    }

    [Fact]
    public async Task UserService_UpdateAsync_ShouldThrow_ArgumentException()
    {
        // Arrange1
        // _userModelTest.Id != input.Id

        // Act1
        var serviceResult = _userService.UpdateAsync("Fake", UserModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert1
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // ***
        // Arrange2
        UserModelTest.Id = string.Empty;

        // Act2
        serviceResult = _userService.UpdateAsync(string.Empty, UserModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

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
        serviceResult = _userService.UpdateAsync(UserModelTest.Id, UserModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

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
        serviceResult = _userService.UpdateAsync(UserModelTest.Id, UserModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

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
        serviceResult = _userService.UpdateAsync(UserModelTest.Id, UserModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert5
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The email cannot be empty.", exception.Message);
        // Restore
        UserModelTest.Email = "Email";
    }

    // ***** ***** ***** DELETE
    [Fact]
    public async Task UserService_DeleteAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.SaveChanges();

        // Act
        await _userService.DeleteAsync("11");
        var actual = _context.TUsers.FirstOrDefault(x => x.UsrId == "1");

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task UserService_DeleteAsync_ShouldThrow_UserNotFound()
    {
        // Arrange
        // No user

        // Act
        var serviceResult = _userService.DeleteAsync("11");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);
    }

    [Fact]
    public async Task UserService_DeleteAsync_ShouldThrow_ArgumentException()
    {
        // Arrange
        // No user

        // Act
        var serviceResult = _userService.DeleteAsync("Unknown");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);
    }
}
