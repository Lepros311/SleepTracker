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

        var repositoryResponse = PagedResponse<List<Sleep>>.Success(sleepData, 1, 10, sleepData.Count);

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

        var failResponse = new PagedResponse<List<Sleep>>(null, 0, 0, 0)
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
}
