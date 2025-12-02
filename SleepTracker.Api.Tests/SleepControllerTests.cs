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
        var response = PagedResponse<List<SleepReadDto>>.Success(new List<SleepReadDto> { new SleepReadDto { Id = 1, DurationHours = "8" } }, 1, 10, 1);

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
        var response = PagedResponse<List<SleepReadDto>>.Fail("Something went wrong");

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
        var sleepDto = new SleepReadDto
        {
            Id = 1,
            Start = DateTime.Now.AddHours(-8).ToString("O"),
            End = DateTime.Now.ToString("O"),
            DurationHours = "8"
        };

        var serviceResponse = new BaseResponse<SleepReadDto>
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
        var returnedSleepDto = okResult.Value as SleepReadDto;
        Assert.IsNotNull(returnedSleepDto);
        Assert.AreEqual(1, returnedSleepDto.Id);
        Assert.AreEqual("8", returnedSleepDto.DurationHours);
    }

    [TestMethod]
    public async Task GetSleepById_ReturnsNotFound_WhenServiceFails()
    {
        // Arrange
        var serviceResponse = new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Fail,
            Message = "Sleep record not found",
            Data = null
        };

        _mockService.Setup(s => s.GetSleepById(99)).ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.GetSleepById(99);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual("Sleep record not found", notFoundResult.Value);
    }

    [TestMethod]
    public async Task CreateSleep_ReturnsCreated_WhenServiceSucceeds()
    {
        // Arrange
        var sleepCreateDto = new SleepCreateDto
        {
            Start = DateTime.Now.AddHours(-8).ToString("O"),
            End = DateTime.Now.ToString("O"),
        };

        var sleepReadDto = new SleepReadDto
        {
            Id = 1,
            Start = sleepCreateDto.Start,
            End = sleepCreateDto.End,
            DurationHours = "8"
        };

        var serviceResponse = new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Success,
            Data = sleepReadDto
        };

        _mockService.Setup(s => s.CreateSleep(It.IsAny<SleepCreateDto>())).ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.CreateSleep(sleepCreateDto);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        var returnedDto = createdResult.Value as SleepReadDto;
        Assert.IsNotNull(returnedDto);
        Assert.AreEqual(1, returnedDto.Id);
    }

    [TestMethod]
    public async Task CreateSleep_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var sleepCreateDto = new SleepCreateDto
        {
            Start = DateTime.Now.ToString("O"),
            End = DateTime.Now.AddHours(8).ToString("O")
        };

        var serviceResponse = new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Fail,
            Message = "Sleep record not created.",
            Data = null
        };

        _mockService.Setup(s => s.CreateSleep(sleepCreateDto)).ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.CreateSleep(sleepCreateDto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        var errorPayload = badRequestResult.Value as string;
        Assert.AreEqual("Sleep record not created.", errorPayload);
    }

    [TestMethod]
    public async Task UpdateSleep_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var sleepUpdateDto = new SleepUpdateDto
        {
            Start = DateTime.Now.AddHours(-7).ToString("O"),
            End = DateTime.Now.ToString("O")
        };

        var updatedSleepDto = new SleepReadDto
        {
            Id = 1,
            Start = sleepUpdateDto.Start,
            End = sleepUpdateDto.End,
            DurationHours = "7"
        };

        var serviceResponse = new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Success,
            Data = updatedSleepDto
        };

        _mockService.Setup(s => s.UpdateSleep(1, sleepUpdateDto)).ReturnsAsync(serviceResponse);

        // Act 
        var result = await _controller.UpdateSleep(1, sleepUpdateDto);

        // Assert
        var noContentResult = result.Result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task UpdateSleep_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        var sleepUpdateDto = new SleepUpdateDto
        {
            Start = DateTime.Now.AddHours(-7).ToString("O"),
            End = DateTime.Now.ToString("O")
        };

        var serviceResponse = new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Fail,
            Message = "Sleep record not updated.",
            Data = null
        };

        _mockService.Setup(s => s.UpdateSleep(1, sleepUpdateDto)).ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.UpdateSleep(1, sleepUpdateDto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        var errorPayload = badRequestResult.Value as string;
        Assert.AreEqual("Sleep record not updated.", errorPayload);
    }

    [TestMethod]
    public async Task DeleteSleep_ReturnsNoContent_WhenServiceReturnsSuccess()
    {
        // Arrange
        var serviceResponse = new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Success
        };

        _mockService.Setup(s => s.DeleteSleep(1)).ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.DeleteSleep(1);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task DeleteSleep_ReturnsBadRequest_WhenServiceReturnsFail()
    {
        // Arrange
        var serviceResponse = new BaseResponse<SleepReadDto>
        {
            Status = ResponseStatus.Fail,
            Message = "Sleep record not found."
        };

        _mockService.Setup(s => s.DeleteSleep(99)).ReturnsAsync(serviceResponse);

        // Act
        var result = await _controller.DeleteSleep(99);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual("Sleep record not found.", notFoundResult.Value);
    }
}
