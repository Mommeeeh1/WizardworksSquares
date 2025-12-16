using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WizardworksSquares.Api.Models;
using WizardworksSquares.Api.Repositories;
using WizardworksSquares.Api.Services;

namespace WizardworksSquares.Tests.Services;

/// <summary>
/// Tests for SquareService focusing on core business logic.
/// Detailed spiral algorithm tests are in SpiralAlgorithmTests.
/// </summary>
public class SquareServiceTests
{
    private readonly ISquareRepository _mockRepository;
    private readonly ILogger<SquareService> _mockLogger;
    private readonly SquareService _service;

    public SquareServiceTests()
    {
        _mockRepository = Substitute.For<ISquareRepository>();
        _mockLogger = Substitute.For<ILogger<SquareService>>();
        _service = new SquareService(_mockRepository, _mockLogger);
        
        // Clear static state before each test
        _mockRepository.ClearAllAsync().Returns(Task.CompletedTask);
        _service.ClearAllSquaresAsync().Wait();
    }

    [Fact]
    public async Task CreateSquareAsync_FirstSquare_ShouldBePlacedAtCenter()
    {
        // Arrange
        _mockRepository.GetAllAsync().Returns(new List<Square>());
        _mockRepository.CreateAsync(Arg.Any<Square>()).Returns(callInfo => callInfo.Arg<Square>());

        // Act
        var result = await _service.CreateSquareAsync();

        // Assert
        result.Row.Should().Be(0);
        result.Column.Should().Be(0);
    }

    [Fact]
    public async Task CreateSquareAsync_AfterClear_ShouldStartFromCenter()
    {
        // Arrange
        var existingSquares = new List<Square>
        {
            new Square { Row = 0, Column = 0 }
        };
        _mockRepository.GetAllAsync().Returns(existingSquares);
        _mockRepository.CreateAsync(Arg.Any<Square>()).Returns(callInfo => callInfo.Arg<Square>());

        // Create second square
        await _service.CreateSquareAsync();

        // Clear
        await _service.ClearAllSquaresAsync();

        // Now repository returns empty
        _mockRepository.GetAllAsync().Returns(new List<Square>());

        // Act - Create first square after clear
        var result = await _service.CreateSquareAsync();

        // Assert - Should be back at center
        result.Row.Should().Be(0);
        result.Column.Should().Be(0);
    }
}
