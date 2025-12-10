using Moq;
using SleepTracker.Api.Models;
using SleepTracker.Api.Repositories;
using SleepTracker.Api.Responses;
using SleepTracker.Api.Services;

namespace SleepTracker.Api.Tests;

[TestClass]
public class SleepServiceTests
{
    private Mock<ISleepRepository> _mockRepository;
    private SleepService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<ISleepRepository>();
        _service = new SleepService(_mockRepository.Object);
    }

    [TestMethod]
    public async Task GetPagedSleeps_ReturnsSuccess_WhenRepositoryReturnsData()
    {
        // Arrange
        var paginationParams = new PaginationParams { Page = 1, PageSize = 10 };
        var sleepData = new List<Sleep>
        {
            new Sleep { Id = 1 }
        };

        var repositoryResponse = PagedResponse<List<Sleep>>.Success(sleepData, 1, 10, sleepData.Count, 1);

        _mockRepository.Setup(r => r.GetPagedSleeps(paginationParams)).ReturnsAsync(repositoryResponse);

        // Act
        var result = await _service.GetPagedSleeps(paginationParams);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.AreEqual(1, result.Data.Count);
        Assert.AreEqual(1, result.PageNumber);
        Assert.AreEqual(10, result.PageSize);
    }

    [TestMethod]
    public async Task GetPagedSleeps_ReturnsFail_WhenRepositoryThrows()
    {
        // Arrange
        var paginationParams = new PaginationParams { Page = 1, PageSize = 10 };

        var failResponse = new PagedResponse<List<Sleep>>(null, 0, 0, 0, 0)
        {
            Status = ResponseStatus.Fail,
            Message = "DB error"
        };

        _mockRepository.Setup(r => r.GetPagedSleeps(paginationParams)).ReturnsAsync(failResponse);

        // Act
        var result = await _service.GetPagedSleeps(paginationParams);

        // Assert 
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("DB error", result.Message);
    }

    [TestMethod]
    public async Task GetSleepById_ReturnsMappedDto_WhenRepositorySucceeds()
    {
        // Arrange
        var sleep = new Sleep
        {
            Id = 1,
            Start = DateTime.Parse("2025-11-24T22:00:00Z"),
            End = DateTime.Parse("2025-11-25T06:00:00Z")
        };

        var repositoryResponse = new BaseResponse<Sleep>
        {
            Status = ResponseStatus.Success,
            Message = "Found",
            Data = sleep
        };

        _mockRepository.Setup(r => r.GetSleepById(1)).ReturnsAsync(repositoryResponse);

        // Act
        var result = await _service.GetSleepById(1);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.AreEqual("Found", result.Message);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(1, result.Data.Id);
        Assert.AreEqual("8", result.Data.DurationHours);
    }

    [TestMethod]
    public async Task GetSleepById_ReturnsFail_WhenRepositoryReturnsNull()
    {
        // Arrange
        var repositoryResponse = new BaseResponse<Sleep>
        {
            Status = ResponseStatus.Fail,
            Message = "Sleep record not found",
            Data = null
        };

        _mockRepository.Setup(r => r.GetSleepById(99)).ReturnsAsync(repositoryResponse);

        // Act
        var result = await _service.GetSleepById(99);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("Sleep record not found", result.Message);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task CreateSleep_ReturnsSuccess_WhenRepositorySucceeds()
    {
        // Arrange
        var sleep = new Sleep
        {
            Id = 1,
            Start = DateTime.Parse("2025-11-28T22:00:00Z"),
            End = DateTime.Parse("2025-11-29T06:00:00Z")
        };

        var repositoryResponse = new BaseResponse<Sleep>
        {
            Status = ResponseStatus.Success,
            Data = sleep
        };

        _mockRepository.Setup(r => r.CreateSleep(It.IsAny<Sleep>())).ReturnsAsync(repositoryResponse);

        var sleepCreateDto = new SleepCreateDto
        {
            Start = sleep.Start.ToString("O"),
            End = sleep.End.ToString("O")
        };

        // Act
        var result = await _service.CreateSleep(sleepCreateDto);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(1, result.Data.Id);
        Assert.AreEqual("8", result.Data.DurationHours);
    }

    [TestMethod]
    public async Task CreateSleep_ReturnsFail_WhenStartIsAfterEnd()
    {
        // Arrange
        var invalidSleepCreateDto = new SleepCreateDto
        {
            Start = DateTime.UtcNow.AddHours(8).ToString("O"),
            End = DateTime.UtcNow.ToString("O")
        };

        var service = new SleepService(_mockRepository.Object);

        // Act
        var result = await service.CreateSleep(invalidSleepCreateDto);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("Start time must be earlier than end time.", result.Message);
        Assert.IsNull(result.Data);

        _mockRepository.Verify(r => r.CreateSleep(It.IsAny<Sleep>()), Times.Never);
    }

    [TestMethod]
    public async Task CreateSleep_ReturnsFail_WhenRepositoryFails()
    {
        // Arrange
        var sleepCreateDto = new SleepCreateDto
        {
            Start = DateTime.Now.AddHours(-8).ToString("O"),
            End = DateTime.Now.ToString("O")
        };

        var repositoryResponse = new BaseResponse<Sleep>
        {
            Status = ResponseStatus.Fail,
            Message = "Sleep record not created.",
            Data = null
        };

        _mockRepository.Setup(r => r.CreateSleep(It.IsAny<Sleep>())).ReturnsAsync(repositoryResponse);

        // Act
        var result = await _service.CreateSleep(sleepCreateDto);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("Sleep record not created.", result.Message);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task UpdateSleep_ReturnsSuccess_WhenRepositorySucceeds()
    {
        // Arrange
        var sleepUpdateDto = new SleepUpdateDto
        {
            Start = DateTime.Now.AddHours(-7).ToString("O"),
            End = DateTime.Now.ToString("O")
        };

        var updatedSleep = new Sleep
        {
            Id = 1,
            Start = DateTime.Parse(sleepUpdateDto.Start),
            End = DateTime.Parse(sleepUpdateDto.End)
        };

        var respositoryResponse = new BaseResponse<Sleep>
        {
            Status = ResponseStatus.Success,
            Data = updatedSleep
        };

        _mockRepository.Setup(r => r.UpdateSleep(It.IsAny<Sleep>())).ReturnsAsync(respositoryResponse);

        // Act
        var result = await _service.UpdateSleep(1, sleepUpdateDto);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(1, result.Data.Id);
        Assert.AreEqual(sleepUpdateDto.Start, result.Data.Start);
        Assert.AreEqual(sleepUpdateDto.End, result.Data.End);
    }

    [TestMethod]
    public async Task UpdateSleep_ReturnsFail_WhenStartIsAfterEnd()
    {
        // Arrange
        var invalidSleepUpdateDto = new SleepUpdateDto
        {
            Start = DateTime.UtcNow.AddHours(8).ToString("O"),
            End = DateTime.UtcNow.ToString("O")
        };

        var service = new SleepService(_mockRepository.Object);

        // Act
        var result = await service.UpdateSleep(1, invalidSleepUpdateDto);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("Start time must be earlier than end time.", result.Message);
        Assert.IsNull(result.Data);

        _mockRepository.Verify(r => r.UpdateSleep(It.IsAny<Sleep>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateSleep_ReturnsFail_WhenRepositoryFails()
    {
        // Arrange
        var sleepUpdateDto = new SleepUpdateDto
        {
            Start = DateTime.Now.AddHours(-7).ToString("O"),
            End = DateTime.Now.ToString("O")
        };

        var repositoryResponse = new BaseResponse<Sleep>
        {
            Status = ResponseStatus.Fail,
            Message = "No changes were saved.",
            Data = null
        };

        _mockRepository.Setup(r => r.UpdateSleep(It.IsAny<Sleep>())).ReturnsAsync(repositoryResponse);

        // Act
        var result = await _service.UpdateSleep(1, sleepUpdateDto);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.IsNull(result.Data);
        Assert.AreEqual("No changes were saved.", result.Message);
    }

    [TestMethod]
    public async Task DeleteSleep_ReturnsSuccess_WhenRepositorySucceeds()
    {
        // Arrange
        var repositoryResponse = new BaseResponse<Sleep>
        {
            Status = ResponseStatus.Success
        };

        _mockRepository.Setup(r => r.DeleteSleep(1)).ReturnsAsync(repositoryResponse);

        var service = new SleepService(_mockRepository.Object);

        // Act
        var result = await service.DeleteSleep(1);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task DeleteSleep_ReturnsFail_WhenRepositoryReturnsNotFound()
    {
        // Arrange
        var repositoryResponse = new BaseResponse<Sleep>
        {
            Status = ResponseStatus.Fail,
            Message = "Sleep record not found."
        };

        _mockRepository.Setup(r => r.DeleteSleep(99)).ReturnsAsync(repositoryResponse);

        var service = new SleepService(_mockRepository.Object);

        // Act
        var result = await service.DeleteSleep(99);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("Sleep record not found.", result.Message);
        Assert.IsNull(result.Data);
    }
}
