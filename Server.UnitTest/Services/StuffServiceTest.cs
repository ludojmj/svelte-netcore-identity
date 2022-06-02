using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Server.DbModels;
using Server.Models;
using Server.Services;
using Server.Services.Interfaces;
using Server.Shared;

namespace Server.UnitTest.Service;

public class StuffServiceTest
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
    private readonly IStuffService _stuffService;

    public StuffServiceTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<StuffDbContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new StuffDbContext(options);
        _context.Database.EnsureCreated();
        var mockAuth = Mock.Of<IUserAuthService>(x => x.GetCurrentUserAsync(It.IsAny<string>()) == Task.FromResult(_dbUser));
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
        _context.SaveChanges();

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
        int requestedPage = 2;
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        _context.SaveChanges();

        // Act
        var serviceResult = await _stuffService.GetListAsync(requestedPage);

        // Assert
        var expected = 1;
        var actual = serviceResult.Page;
        Assert.Equal(expected, actual);
    }

    // ***** ***** ***** SEARCH
    [Fact]
    public async Task StuffService_SearchListAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        _context.SaveChanges();

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
        _context.SaveChanges();

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
        _context.SaveChanges();

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
        DatumModelTest.User = UserModelTest;
    }

    [Fact]
    public async Task StuffService_CreateAsync_ShouldReturn_ArgumentException()
    {
        // Arrange
        DatumModelTest.Label = string.Empty;
        _context.Add(_dbUser);
        _context.SaveChanges();

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
        _context.SaveChanges();

        // Act
        var serviceResult = await _stuffService.ReadAsync("1");

        // Assert
        var expected = DatumModelTest.Id;
        var actual = serviceResult.Id;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task StuffService_ReadAsync_ShouldThrow_NotFoundException()
    {
        // Arrange
        // No stuff

        // Act
        var serviceResult = _stuffService.ReadAsync("2");
        var exception = await Record.ExceptionAsync(() => serviceResult);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<NotFoundException>(exception);
        Assert.Equal("Stuff not found.", exception.Message);
    }

    // ***** ***** ***** UPDATE
    [Fact]
    public async Task StuffService_UpdateAsync_ShouldReturn_Ok()
    {
        // Arrange
        _context.Add(_dbUser);
        _context.Add(_dbStuff);
        _context.SaveChanges();

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
        _context.SaveChanges();
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
        _context.SaveChanges();

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
        _context.SaveChanges();
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
