using Microsoft.AspNetCore.Mvc;
using Moq;
using SleepTracker.Api.Controllers;
using SleepTracker.Api.Models;
using SleepTracker.Api.Responses;
using SleepTracker.Api.Services;

namespace SleepTracker.Api.Tests;

[TestClass]
public class SleepControllerTests
{
    private Mock<ISleepService> _mockService;
    private SleepController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<ISleepService>();
        _controller = new SleepController(_mockService.Object);
    }

    [TestMethod]
    public async Task GetPagedSleeps_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var paginationParams = new PaginationParams { Page = 1, PageSize = 10 };
        var response = PagedResponse<List<SleepDto>>.Success(new List<SleepDto> { new SleepDto { Id = 1, DurationHours = "8" } }, 1, 10, 1);

        _mockService.Setup(s => s.GetPagedSleeps(paginationParams)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetPagedSleeps(paginationParams);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(response, okResult.Value);
    }

    [TestMethod]
    public async Task GetPagedSleeps_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var paginationParams = new PaginationParams { Page = 1, PageSize = 10 };
        var response = PagedResponse<List<SleepDto>>.Fail("Something went wrong");

        _mockService.Setup(s => s.GetPagedSleeps(paginationParams)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetPagedSleeps(paginationParams);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Something went wrong", badRequestResult.Value);
    }

    [TestMethod]
    public async Task GetSleepById_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var sleepDto = new SleepDto
        {
            Id = 1,
            Start = DateTime.Now.AddHours(-8).ToString("O"),
            End = DateTime.Now.ToString("O"),
            DurationHours = "8"
        };

        var serviceResponse = new BaseResponse<SleepDto>
        {
            Status = ResponseStatus.Success,
            Message = "Found",
            Data = sleepDto
        };

        _mockService.Setup(s => s.GetSleepById(1)).ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.GetSleepById(1);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var returnedSleepDto = okResult.Value as SleepDto;
        Assert.IsNotNull(returnedSleepDto);
        Assert.AreEqual(1, returnedSleepDto.Id);
        Assert.AreEqual("8", returnedSleepDto.DurationHours);
    }
}
