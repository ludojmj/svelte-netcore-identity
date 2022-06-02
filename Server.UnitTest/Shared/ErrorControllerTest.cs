using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Server.Shared;

namespace Server.UnitTest.Shared;

public class ErrorControllerTest
{
    [Fact]
    public void ErrorController_NotFoundObjectResult()
    {
        // Arrange
        var mockEnv = Mock.Of<IWebHostEnvironment>();
        var mockLog = Mock.Of<ILogger<ErrorController>>();
        var mockException = Mock.Of<IExceptionHandlerFeature>(x => x.Error == new NotFoundException("Not found"));

        var context = new DefaultHttpContext();
        context.Features.Set<IExceptionHandlerFeature>(mockException);

        var controller = new ErrorController()
        {
            ControllerContext = new ControllerContext() { HttpContext = context }
        };

        // Act
        IActionResult actionResult = controller.Error(mockEnv, mockLog);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var contentResult = Assert.IsType<ErrorModel>(notFoundResult.Value);
        var expected = "Not found";
        var actual = contentResult.Error;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ErrorHandlerFilter_BadRequestObjectResult_Development()
    {
        // Arrange
        var mockEnv = Mock.Of<IWebHostEnvironment>(x => x.EnvironmentName == "Development");
        var mockLog = Mock.Of<ILogger<ErrorController>>();
        var mockException = Mock.Of<IExceptionHandlerFeature>(x => x.Error == new ArgumentException("Should be displayed"));

        var context = new DefaultHttpContext();
        context.Features.Set<IExceptionHandlerFeature>(mockException);

        var controller = new ErrorController()
        {
            ControllerContext = new ControllerContext() { HttpContext = context }
        };

        // Act
        IActionResult actionResult = controller.Error(mockEnv, mockLog);

        // Assert
        var notFoundResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var contentResult = Assert.IsType<ErrorModel>(notFoundResult.Value);
        var expected = "Should be displayed";
        var actual = contentResult.Error;
        Assert.Equal(expected, actual);
    }


    [Fact]
    public void ErrorHandlerFilter_BadRequestObjectResult_Production()
    {
        // Arrange
        var mockEnv = Mock.Of<IWebHostEnvironment>(x => x.EnvironmentName == "Production");
        var mockLog = Mock.Of<ILogger<ErrorController>>();
        var mockException = Mock.Of<IExceptionHandlerFeature>(x => x.Error == new ArgumentException("Should not be displayed"));

        var context = new DefaultHttpContext();
        context.Features.Set<IExceptionHandlerFeature>(mockException);

        var controller = new ErrorController()
        {
            ControllerContext = new ControllerContext() { HttpContext = context }
        };

        // Act
        IActionResult actionResult = controller.Error(mockEnv, mockLog);

        // Assert
        var notFoundResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var contentResult = Assert.IsType<ErrorModel>(notFoundResult.Value);
        var expected = "An error occured. Please try again later.";
        var actual = contentResult.Error;
        Assert.Equal(expected, actual);
    }
}
