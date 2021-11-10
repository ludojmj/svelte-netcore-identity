using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Server.Controllers;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.UnitTest.Controllers
{
    public class UserControllerTest
    {
        private static readonly UserModel TestUser = new UserModel()
        {
            Id = "11",
            Name = "GivenName FamilyName",
            GivenName = "GivenName",
            FamilyName = "FamilyName",
            Email = "Email"
        };

        private static readonly DirectoryModel TestDirectory = new DirectoryModel
        {
            UserList = new Collection<UserModel>
            {
                TestUser
            }
        };

        // ***** ***** ***** LIST
        [Fact]
        public async Task UserController_GetUserList_ShouldReturn_Ok()
        {
            // Arrange
            var mockUserRepo = Mock.Of<IUserRepo>(x => x.GetListAsync(1) == Task.FromResult(TestDirectory));
            var controller = new UserController(mockUserRepo);
            int existingPage = 1;

            // Act
            IActionResult actionResult = await controller.GetList(existingPage, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = Assert.IsType<DirectoryModel>(okResult.Value);
            var expected = TestUser.Id;
            var actual = contentResult.UserList.ToArray()[0].Id;
            Assert.Equal(expected, actual);
        }

        // ***** ***** ***** SEARCH
        [Fact]
        public async Task UserController_SearchUserList_ShouldReturn_Ok()
        {
            // Arrange
            var mockUserRepo = Mock.Of<IUserRepo>(x => x.SearchListAsync("foo") == Task.FromResult(TestDirectory));
            var controller = new UserController(mockUserRepo);

            // Act
            IActionResult actionResult = await controller.GetList(0, "foo");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = Assert.IsType<DirectoryModel>(okResult.Value);
            var expected = TestUser.Id;
            var actual = contentResult.UserList.ToArray()[0].Id;
            Assert.Equal(expected, actual);
        }

        // ***** ***** ***** CREATE
        [Fact]
        public async Task UserController_Create_ShouldReturnCreated()
        {
            // Arrange
            var mockUserRepo = Mock.Of<IUserRepo>(x => x.CreateAsync(TestUser) == Task.FromResult(TestUser));
            var controller = new UserController(mockUserRepo);

            // Act
            IActionResult actionResult = await controller.Create(TestUser);

            // Assert
            var okResult = Assert.IsType<CreatedAtActionResult>(actionResult);
            var contentResult = Assert.IsType<UserModel>(okResult.Value);
            Assert.Equal(TestUser.Id, contentResult.Id);
        }

        // ***** ***** ***** READ SINGLE
        [Fact]
        public async Task UserController_Read_ShouldReturn_Ok()
        {
            // Arrange
            var mockUserRepo = Mock.Of<IUserRepo>(x => x.ReadAsync("1") == Task.FromResult(TestUser));
            var controller = new UserController(mockUserRepo);

            // Act
            IActionResult actionResult = await controller.Read("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = Assert.IsType<UserModel>(okResult.Value);
            var expected = TestUser.Id;
            var actual = contentResult.Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UserController_Read_ShouldReturn_Null()
        {
            // Arrange
            var mockUserRepo = Mock.Of<IUserRepo>();
            var controller = new UserController(mockUserRepo);

            // Act
            IActionResult actionResult = await controller.Read("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = okResult.Value;
            UserModel expected = null;
            var actual = contentResult;
            Assert.Equal(expected, actual);
        }

        // ***** ***** ***** UPDATE
        [Fact]
        public async Task UserController_UpdateUser_ShouldReturn_Ok()
        {
            // Arrange
            var mockUserRepo = Mock.Of<IUserRepo>(x => x.UpdateAsync("1", TestUser) == Task.FromResult(TestUser));
            var controller = new UserController(mockUserRepo);
            string existingId = "1";

            // Act
            IActionResult actionResult = await controller.Update(existingId, TestUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = Assert.IsType<UserModel>(okResult.Value);
            Assert.Equal(TestUser.Id, contentResult.Id);
        }

        // ***** ***** ***** DELETE
        [Fact]
        public async Task UserController_DeleteUser_ShouldReturnNoContent()
        {
            // Arrange
            var mockUserRepo = Mock.Of<IUserRepo>();
            var controller = new UserController(mockUserRepo);
            string badId = "2";

            // Act
            IActionResult actionResult = await controller.Delete(badId);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
        }
    }
}
