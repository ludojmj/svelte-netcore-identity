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
    public class StuffControllerTest
    {
        private static readonly UserModel CurrentUserModelTest = new UserModel()
        {
            Id = "11",
            Name = "GivenName FamilyName",
            GivenName = "GivenName",
            FamilyName = "FamilyName",
            Email = "Email"
        };

        private static readonly DatumModel TestDatum = new DatumModel
        {
            Id = "1",
            Label = "Label",
            Description = "Description",
            OtherInfo = "OtherInfo",
            User = CurrentUserModelTest
        };

        private static readonly StuffModel TestStuff = new StuffModel
        {
            DatumList = new Collection<DatumModel>
            {
                TestDatum
            }
        };

        // ***** ***** ***** LIST
        [Fact]
        public async Task StuffController_GetStuffList_ShouldReturn_Ok()
        {
            // Arrange
            var mockStuffRepo = Mock.Of<IStuffRepo>(x => x.GetListAsync(1) == Task.FromResult(TestStuff));
            var controller = new StuffController(mockStuffRepo);
            int existingPage = 1;

            // Act
            IActionResult actionResult = await controller.GetList(existingPage, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = Assert.IsType<StuffModel>(okResult.Value);
            var expected = TestDatum.Id;
            var actual = contentResult.DatumList.ToArray()[0].Id;
            Assert.Equal(expected, actual);
        }

        // ***** ***** ***** SEARCH
        [Fact]
        public async Task StuffController_SearchStuffList_ShouldReturn_Ok()
        {
            // Arrange
            var mockStuffRepo = Mock.Of<IStuffRepo>(x => x.SearchListAsync("foo") == Task.FromResult(TestStuff));
            var controller = new StuffController(mockStuffRepo);

            // Act
            IActionResult actionResult = await controller.GetList(0, "foo");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = Assert.IsType<StuffModel>(okResult.Value);
            var expected = TestDatum.Id;
            var actual = contentResult.DatumList.ToArray()[0].Id;
            Assert.Equal(expected, actual);
        }

        // ***** ***** ***** CREATE
        [Fact]
        public async Task StuffController_Create_ShouldReturnCreated()
        {
            // Arrange
            var mockStuffRepo = Mock.Of<IStuffRepo>(x => x.CreateAsync(TestDatum) == Task.FromResult(TestDatum));
            var controller = new StuffController(mockStuffRepo);

            // Act
            IActionResult actionResult = await controller.Create(TestDatum);

            // Assert
            var okResult = Assert.IsType<CreatedAtActionResult>(actionResult);
            var contentResult = Assert.IsType<DatumModel>(okResult.Value);
            Assert.Equal(TestDatum.Id, contentResult.Id);
        }

        // ***** ***** ***** READ SINGLE
        [Fact]
        public async Task StuffController_Read_ShouldReturn_Ok()
        {
            // Arrange
            var mockStuffRepo = Mock.Of<IStuffRepo>(x => x.ReadAsync("1") == Task.FromResult(TestDatum));
            var controller = new StuffController(mockStuffRepo);

            // Act
            IActionResult actionResult = await controller.Read("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = Assert.IsType<DatumModel>(okResult.Value);
            var expected = TestDatum.Id;
            var actual = contentResult.Id;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task StuffController_Read_ShouldReturn_Null()
        {
            // Arrange
            var mockUserRepo = Mock.Of<IStuffRepo>();
            var controller = new StuffController(mockUserRepo);

            // Act
            IActionResult actionResult = await controller.Read("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = okResult.Value;
            StuffModel expected = null;
            var actual = contentResult;
            Assert.Equal(expected, actual);
        }

        // ***** ***** ***** UPDATE
        [Fact]
        public async Task StuffController_UpdateStuff_ShouldReturn_Ok()
        {
            // Arrange
            var mockStuffRepo = Mock.Of<IStuffRepo>(x => x.UpdateAsync("1", TestDatum) == Task.FromResult(TestDatum));
            var controller = new StuffController(mockStuffRepo);
            string existingId = "1";

            // Act
            IActionResult actionResult = await controller.Update(existingId, TestDatum);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var contentResult = Assert.IsType<DatumModel>(okResult.Value);
            Assert.Equal(TestDatum.Id, contentResult.Id);
        }

        // ***** ***** ***** DELETE
        [Fact]
        public async Task StuffController_DeleteStuff_ShouldReturnNoContent()
        {
            // Arrange
            var mockStuffRepo = Mock.Of<IStuffRepo>();
            var controller = new StuffController(mockStuffRepo);
            string badId = "2";

            // Act
            IActionResult actionResult = await controller.Delete(badId);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
        }
    }
}
