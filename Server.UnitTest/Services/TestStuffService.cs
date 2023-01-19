using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Server.DbModels;
using Server.Models;
using Server.Services;
using Server.Services.Interfaces;
using Xunit;

namespace Server.UnitTest.Services;

public class TestStuffService
{
    private static readonly UserModel TestUserModel = new()
    {
        Id = "11",
        Name = "GivenName FamilyName",
        GivenName = "GivenName",
        FamilyName = "FamilyName",
        Email = "Email"
    };

    private static readonly DatumModel DatumModelTest = new()
    {
        Id = "1",
        Label = "Label",
        Description = "Description",
        OtherInfo = "OtherInfo",
        User = TestUserModel
    };

    private readonly TUser _dbUser = new()
    {
        UsrId = "11",
        UsrName = "GivenName FamilyName",
        UsrGivenName = "GivenName",
        UsrFamilyName = "FamilyName",
        UsrEmail = "Email"
    };

    private readonly TStuff _dbStuff = new()
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
    private readonly IStuffService _stuffService;

    public TestStuffService()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<StuffDbContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new StuffDbContext(options);
        _context.Database.EnsureCreated();
        var mockAuth = Mock.Of<IUserAuthService>(x => x.GetCurrentUser(It.IsAny<string>()) == _dbUser);
        _stuffService = new StuffService(_context, mockAuth);
    }

    [Fact]
    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }

    // ***** ***** ***** LIST
    [Fact]
    public async Task StuffService_GetListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        await _context.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.GetListAsync(1);

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_GetListAsync_ShouldReturn_PageOne()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        await _context.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.GetListAsync(2);

        // Assert
        var actual = serviceResult.Page;
        Assert.Equal(1, actual);
    }

    // ***** ***** ***** SEARCH
    [Fact]
    public async Task StuffService_SearchListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        await _context.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.SearchListAsync("LABEL");

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.DatumList.ToArray()[0].Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_SearchListAsync_ShouldThrow_ArgumentException()
    {
        // Arrange
        _context.Add(_dbUser);
        var dbStuffList = new List<TStuff>();
        for (int idx = 0; idx < 7; idx++)
        {
            var tmpStuff = new TStuff
            {
                StfId = _dbStuff.StfId + (idx + 1),
                StfUserId = _dbStuff.StfUserId,
                StfLabel = _dbStuff.StfLabel
            };
            dbStuffList.Add(tmpStuff);
        }

        _context.AddRange(dbStuffList);
        await _context.SaveChangesAsync();

        // Act
        var serviceResult = _stuffService.SearchListAsync("LABEL");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Too many results. Please narrow your search.", exception.Message);
    }

    // ***** ***** ***** CREATE
    [Fact]
    public async Task StuffService_CreateAsync_ShouldReturn_Ok()
    {
        // Arrange1
        // Existing user
        _context.Add(_dbUser);
        await _context.SaveChangesAsync();

        // Act1
        var serviceResult = await _stuffService.CreateAsync(DatumModelTest);

        // Assert1
        int actual = serviceResult.Id.Count(x => x == '-');
        int expected = 4;
        Assert.Equal(expected, actual);

        // ***
        // Arrange2
        // Creating user at the same time as stuff
        DatumModelTest.User = null;

        // Act2
        serviceResult = await _stuffService.CreateAsync(DatumModelTest);

        // Assert2
        actual = serviceResult.Id.Count(x => x == '-');
        expected = 4;
        Assert.Equal(expected, actual);

        // Restore
        DatumModelTest.User = TestUserModel;
    }

    [Fact]
    public async Task StuffService_CreateAsync_ShouldReturn_ArgumentException()
    {
        // Arrange
        DatumModelTest.Label = string.Empty;
        _context.Add(_dbUser);
        await _context.SaveChangesAsync();

        // Act
        var serviceResult = _stuffService.CreateAsync(DatumModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The label cannot be empty.", exception.Message);

        // Restore
        DatumModelTest.Label = "Label";
    }

    // ***** ***** ***** READ SINGLE
    [Fact]
    public async Task StuffService_ReadAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        await _context.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.ReadAsync("1");

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_ReadAsync_ShouldThrow_KeyNotFoundException()
    {
        // Arrange
        // No stuff

        // Act
        var serviceResult = _stuffService.ReadAsync("2");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<KeyNotFoundException>(exception);
        Assert.Equal("Stuff not found.", exception.Message);
    }

    // ***** ***** ***** UPDATE
    [Fact]
    public async Task StuffService_UpdateAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        await _context.SaveChangesAsync();

        // Act
        var serviceResult = await _stuffService.UpdateAsync("1", DatumModelTest);

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_UpdateAsync_ShouldThrow_ArgumentException()
    {
        // Arrange1
        // _datumModelTest.Id != input.Id

        // Act1
        var serviceResult = _stuffService.UpdateAsync("2", DatumModelTest);
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert1
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // ***
        // Arrange2
        DatumModelTest.Id = "StuffNotFound";

        // Act2
        serviceResult = _stuffService.UpdateAsync("StuffNotFound", DatumModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

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
        serviceResult = _stuffService.UpdateAsync(DatumModelTest.Id, DatumModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

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
        await _context.SaveChangesAsync();
        _dbUser.UsrId = "11";

        // Act4
        serviceResult = _stuffService.UpdateAsync("1", DatumModelTest);
        exception = await Record.ExceptionAsync(() => serviceResult);

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
    public async Task StuffService_DeleteAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        await _context.SaveChangesAsync();

        // Act
        await _stuffService.DeleteAsync("1");
        var actual = _context.TStuffs.FirstOrDefault(x => x.StfId == "1");

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task StuffService_DeleteAsync_ShouldThrow_ArgumentException()
    {
        // Arrange1
        // No stuff

        // Act1
        var serviceResult = _stuffService.DeleteAsync("2");
        var exception = await Record.ExceptionAsync(() => serviceResult);

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
        await _context.SaveChangesAsync();
        _dbUser.UsrId = "11";

        // Act2
        serviceResult = _stuffService.DeleteAsync("1");
        exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert2
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Corrupted data.", exception.Message);

        // Restore
        _dbStuff.StfUserId = "11";
        _dbStuff.StfUser.UsrId = "11";
    }
}
