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
        Assert.Contains("Object reference", result.Message);
    }

    [TestMethod]
    public async Task GetSleepById_ReturnsSuccess_WhenRecordExists()
    {
        // Act
        var result = await _repository.GetSleepById(1);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(1, result.Data.Id);
    }

    [TestMethod]
    public async Task GetSleepById_ReturnsFail_WhenRecordDoesNotExist()
    {
        // Act
        var result = await _repository.GetSleepById(99);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("Sleep record not found.", result.Message);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task GetSleepById_ReturnsFail_WhenDbThrows()
    {
        // Arrange
        var badRepository = new SleepRepository(null!);

        // Act
        var result = await badRepository.GetSleepById(1);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.Contains("Object reference", result.Message);
    }

    [TestMethod]
    public async Task CreateSleep_ReturnsSuccess_WhenEntityIsValid()
    {
        // Arrange
        var sleep = new Sleep
        {
            Start = DateTime.Parse("2025-11-28T22:00:00Z"),
            End = DateTime.Parse("2025-11-29T06:00:00Z")
        };

        // Act
        var result = await _repository.CreateSleep(sleep);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.IsNotNull(result.Data);
        Assert.IsGreaterThan(0, result.Data.Id);
        Assert.AreEqual(sleep.Start, result.Data.Start);
        Assert.AreEqual(sleep.End, result.Data.End);
    }

    [TestMethod]
    public async Task CreateSleep_ReturnsFail_WhenDbContextThrows()
    {
        // Arrange
        var badRepository = new SleepRepository(null);
        var sleep = new Sleep
        {
            Start = DateTime.Now,
            End = DateTime.Now.AddHours(8)
        };

        // Act
        var result = await badRepository.CreateSleep(sleep);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.IsNull(result.Data);
        Assert.Contains("Object reference", result.Message);
    }

    [TestMethod]
    public async Task UpdateSleep_ReturnsSuccess_WhenEntityIsUpdated()
    {
        // Arrange
        var updatedSleep = new Sleep
        {
            Id = 1,
            Start = DateTime.Now.AddHours(-7),
            End = DateTime.Now
        };

        // Act
        var result = await _repository.UpdateSleep(updatedSleep);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(updatedSleep.Id, result.Data.Id);
        Assert.AreEqual(updatedSleep.Start, result.Data.Start);
        Assert.AreEqual(updatedSleep.End, result.Data.End);
    }

    [TestMethod]
    public async Task UpdateSleep_ReturnsFail_WhenEntityDoesNotExist()
    {
        // Arrange
        var updatedSleep = new Sleep
        {
            Id = 99,
            Start = DateTime.Now.AddHours(-7),
            End = DateTime.Now
        };

        // Act
        var result = await _repository.UpdateSleep(updatedSleep);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("No sleep record with that ID found.", result.Message);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task UpdateSleep_ReturnsFail_WhenDbContextThrows()
    {
        // Arrange
        var badRepository = new SleepRepository(null!);
        var updatedSleep = new Sleep
        {
            Id = 1,
            Start = DateTime.Now.AddHours(-7),
            End = DateTime.Now
        };

        // Act
        var result = await badRepository.UpdateSleep(updatedSleep);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.IsNull(result.Data);
        Assert.Contains("Error in SleepRepository UpdateSleep", result.Message);
    }

    [TestMethod]
    public async Task DeleteSleep_ReturnsSuccess_WhenEntityExists()
    {
        // Act
        var result = await _repository.DeleteSleep(1);

        // Assert
        Assert.AreEqual(ResponseStatus.Success, result.Status);
        var sleep = await _dbContext.Sleeps.FindAsync(1);
        Assert.IsNotNull(sleep);
        Assert.IsTrue(sleep.IsDeleted);
    }

    [TestMethod]
    public async Task DeleteSleep_ReturnsFail_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _repository.DeleteSleep(99);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.AreEqual("Sleep record not found.", result.Message);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task DeleteSleep_ReturnsFail_WhenDbContextThrows()
    {
        // Arrange
        var badRepository = new SleepRepository(null!);

        // Act
        var result = await badRepository.DeleteSleep(1);

        // Assert
        Assert.AreEqual(ResponseStatus.Fail, result.Status);
        Assert.Contains("Error in SleepRepository DeleteSleep", result.Message);
    }
}
