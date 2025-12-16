using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WizardworksSquares.Api.Models;
using WizardworksSquares.Api.Repositories;
using WizardworksSquares.Api.Services;

namespace WizardworksSquares.Tests;

/// <summary>
/// Tests specifically for the spiral algorithm logic.
/// Ensures positions are unique and follow spiral pattern.
/// </summary>
public class SpiralAlgorithmTests
{
    [Theory]
    [InlineData(1)]   // 1x1 grid
    [InlineData(4)]   // 2x2 grid
    [InlineData(9)]   // 3x3 grid
    [InlineData(16)]  // 4x4 grid
    [InlineData(25)]  // 5x5 grid
    public async Task SpiralAlgorithm_ShouldHaveUniquePositions(int squareCount)
    {
        // Arrange
        var mockRepository = Substitute.For<ISquareRepository>();
        var mockLogger = Substitute.For<ILogger<SquareService>>();
        var service = new SquareService(mockRepository, mockLogger);
        
        // Clear static state
        mockRepository.ClearAllAsync().Returns(Task.CompletedTask);
        await service.ClearAllSquaresAsync();

        var positions = new HashSet<(int row, int col)>();
        var squares = new List<Square>();

        // Act - Create squares one by one
        for (int i = 0; i < squareCount; i++)
        {
            mockRepository.GetAllAsync().Returns(squares.ToList());
            mockRepository.CreateAsync(Arg.Any<Square>()).Returns(callInfo => 
            {
                var square = callInfo.Arg<Square>();
                squares.Add(square);
                return square;
            });

            var result = await service.CreateSquareAsync();
            positions.Add((result.Row, result.Column));
        }

        // Assert - All positions should be unique
        positions.Should().HaveCount(squareCount, "each square should have a unique position");
    }

    [Fact]
    public async Task SpiralAlgorithm_GridSize_ShouldFollowCeilSqrtFormula()
    {
        // Arrange
        var mockRepository = Substitute.For<ISquareRepository>();
        var mockLogger = Substitute.For<ILogger<SquareService>>();
        var service = new SquareService(mockRepository, mockLogger);

        var testCases = new[]
        {
            (squareCount: 1, expectedGridSize: 1),   // ceil(sqrt(1)) = 1
            (squareCount: 2, expectedGridSize: 2),   // ceil(sqrt(2)) = 2
            (squareCount: 4, expectedGridSize: 2),   // ceil(sqrt(4)) = 2
            (squareCount: 5, expectedGridSize: 3),   // ceil(sqrt(5)) = 3
            (squareCount: 9, expectedGridSize: 3),   // ceil(sqrt(9)) = 3
            (squareCount: 10, expectedGridSize: 4),  // ceil(sqrt(10)) = 4
        };

        foreach (var (squareCount, expectedGridSize) in testCases)
        {
            // Reset service state
            await service.ClearAllSquaresAsync();

            var squares = new List<Square>();
            mockRepository.GetAllAsync().Returns(squares);
            mockRepository.CreateAsync(Arg.Any<Square>()).Returns(callInfo => 
            {
                var square = callInfo.Arg<Square>();
                squares.Add(square);
                return square;
            });

            // Act - Create squares
            for (int i = 0; i < squareCount; i++)
            {
                await service.CreateSquareAsync();
            }

            // Assert - All positions should fit within expected grid size
            var maxRow = squares.Max(s => s.Row);
            var maxCol = squares.Max(s => s.Column);

            maxRow.Should().BeLessThan(expectedGridSize, 
                $"for {squareCount} squares, max row should fit in {expectedGridSize}x{expectedGridSize} grid");
            maxCol.Should().BeLessThan(expectedGridSize, 
                $"for {squareCount} squares, max column should fit in {expectedGridSize}x{expectedGridSize} grid");
        }
    }

    [Fact]
    public async Task SpiralAlgorithm_FirstNinePositions_ShouldFollowExpectedPattern()
    {
        // Arrange
        var mockRepository = Substitute.For<ISquareRepository>();
        var mockLogger = Substitute.For<ILogger<SquareService>>();
        var service = new SquareService(mockRepository, mockLogger);
        
        // Clear static state
        mockRepository.ClearAllAsync().Returns(Task.CompletedTask);
        await service.ClearAllSquaresAsync();

        var squares = new List<Square>();
        mockRepository.GetAllAsync().Returns(squares);
        mockRepository.CreateAsync(Arg.Any<Square>()).Returns(callInfo => 
        {
            var square = callInfo.Arg<Square>();
            squares.Add(square);
            return square;
        });

        // Expected positions for first 9 squares in spiral pattern
        var expectedPositions = new List<(int row, int col)>
        {
            (0, 0), // Square 1: center
            (0, 1), // Square 2: right
            (1, 1), // Square 3: down
            (1, 0), // Square 4: left
            (0, 2), // Square 5: start of ring 3
            (1, 2), // Square 6
            (2, 2), // Square 7
            (2, 1), // Square 8
            (2, 0), // Square 9
        };

        // Act - Create 9 squares
        for (int i = 0; i < 9; i++)
        {
            await service.CreateSquareAsync();
        }

        // Assert
        for (int i = 0; i < 9; i++)
        {
            squares[i].Row.Should().Be(expectedPositions[i].row, 
                $"square {i + 1} should be at expected row position");
            squares[i].Column.Should().Be(expectedPositions[i].col, 
                $"square {i + 1} should be at expected column position");
        }
    }

    [Fact]
    public async Task SpiralAlgorithm_PositionsShouldNotChange_AfterClearAndRestart()
    {
        // Arrange
        var mockRepository = Substitute.For<ISquareRepository>();
        var mockLogger = Substitute.For<ILogger<SquareService>>();
        var service = new SquareService(mockRepository, mockLogger);

        // First run - create 5 squares
        var firstRunSquares = new List<Square>();
        mockRepository.GetAllAsync().Returns(firstRunSquares);
        mockRepository.CreateAsync(Arg.Any<Square>()).Returns(callInfo => 
        {
            var square = callInfo.Arg<Square>();
            firstRunSquares.Add(square);
            return square;
        });

        for (int i = 0; i < 5; i++)
        {
            await service.CreateSquareAsync();
        }

        var firstRunPositions = firstRunSquares.Select(s => (s.Row, s.Column)).ToList();

        // Clear
        await service.ClearAllSquaresAsync();

        // Second run - create 5 squares again
        var secondRunSquares = new List<Square>();
        mockRepository.GetAllAsync().Returns(secondRunSquares);
        mockRepository.CreateAsync(Arg.Any<Square>()).Returns(callInfo => 
        {
            var square = callInfo.Arg<Square>();
            secondRunSquares.Add(square);
            return square;
        });

        for (int i = 0; i < 5; i++)
        {
            await service.CreateSquareAsync();
        }

        var secondRunPositions = secondRunSquares.Select(s => (s.Row, s.Column)).ToList();

        // Assert - Positions should be identical
        secondRunPositions.Should().BeEquivalentTo(firstRunPositions, options => options.WithStrictOrdering(),
            "spiral should restart from the same pattern after clear");
    }
}
