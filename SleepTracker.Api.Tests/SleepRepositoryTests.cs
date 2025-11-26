using Microsoft.EntityFrameworkCore;
using SleepTracker.Api.Data;
using SleepTracker.Api.Models;
using SleepTracker.Api.Repositories;
using SleepTracker.Api.Responses;

namespace SleepTracker.Api.Tests;

[TestClass]
public class SleepRepositoryTests
{
    private SleepTrackerDbContext _dbContext;
    private SleepRepository _repository;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SleepTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        _dbContext = new SleepTrackerDbContext(options);
        _repository = new SleepRepository(_dbContext);

        _dbContext.Sleeps.AddRange(new List<Sleep>
        {
            new Sleep { Id = 1, Start = DateTime.Now.AddHours(-8), End = DateTime.Now },
            new Sleep { Id = 2, Start = DateTime.Now.AddHours(-7), End = DateTime.Now }
        });

        _dbContext.SaveChanges();
    }

    [TestMethod]
    public async Task GetPagedSleeps_ReturnsSuccess_WithCorrectPagination()
    {
        // Arrange
        var paginationParams = new PaginationParams { Page = 1, PageSize = 1 };

        // Act
        var result = await _repository.GetPagedSleeps(paginationParams);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.AreEqual(1, result.Data.Count);
        Assert.AreEqual(1, result.PageNumber);
        Assert.AreEqual(1, result.PageSize);
        Assert.AreEqual(2, result.TotalRecords);
    }

    [TestMethod]
    public async Task GetPagedSleeps_ReturnsFail_WhenDbThrows()
    {
        // Arrange
        var badRepository = new SleepRepository(null!);
        var paginationParams = new PaginationParams { Page = 1, PageSize = 10 };

        // Act
        var result = await badRepository.GetPagedSleeps(paginationParams);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.IsTrue(result.Message.Contains("Object reference"));
    }
}
