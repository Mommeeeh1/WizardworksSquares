using WizardworksSquares.Api.Constants;
using WizardworksSquares.Api.DTO;
using WizardworksSquares.Api.Models;
using WizardworksSquares.Api.Repositories;

namespace WizardworksSquares.Api.Services;

/// <summary>
/// Service implementation for square business logic.
/// Handles square positioning using a spiral grid algorithm and color assignment.
/// </summary>
public class SquareService : ISquareService
{
    private readonly ISquareRepository _repository;
    private readonly ILogger<SquareService> _logger;
    private static string? _lastColor = null;
    private static readonly Random _random = new Random();

    public SquareService(ISquareRepository repository, ILogger<SquareService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<SquareDto>> GetAllSquaresAsync()
    {
        var squares = await _repository.GetAllAsync();
        return squares.Select(s => new SquareDto(s.Id, s.Row, s.Column, s.Color, s.CreatedAt)).ToList();
    }

    private static List<(int row, int col)> _expandingSpiral = new List<(int, int)>();
    private static HashSet<(int row, int col)> _spiralSet = new HashSet<(int, int)>();
    private static int _currentSpiralSize = 0;

    public async Task<SquareDto> CreateSquareAsync()
    {
        try
        {
            var allSquares = await _repository.GetAllAsync();
            var squareIndex = allSquares.Count; // 0-based index for the new square
            
            // Ensure spiral is large enough for this square
            var neededSize = (int)Math.Ceiling(Math.Sqrt(squareIndex + 1));
            ExpandSpiralTo(neededSize);
            
            var (row, column) = _expandingSpiral[squareIndex];
            
            // Select random color (different from last)
            string color;
            do
            {
                color = SquareConstants.AvailableColors[_random.Next(SquareConstants.AvailableColors.Length)];
            } while (color == _lastColor && SquareConstants.AvailableColors.Length > 1);
            
            _lastColor = color;
            
            var square = new Square
            {
                Row = row,
                Column = column,
                Color = color,
                CreatedAt = DateTime.UtcNow
            };
            
            var savedSquare = await _repository.CreateAsync(square);
            
            _logger.LogDebug("Successfully created square at position ({Row}, {Column}) with color {Color}", 
                savedSquare.Row, savedSquare.Column, savedSquare.Color);
            
            return new SquareDto(savedSquare.Id, savedSquare.Row, savedSquare.Column, savedSquare.Color, savedSquare.CreatedAt);
        }
        catch (InvalidOperationException)
        {
            // Re-throw business logic exceptions
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating square");
            throw new InvalidOperationException("An unexpected error occurred while creating the square", ex);
        }
    }

    /// <summary>
    /// Expands the spiral grid to accommodate the target size.
    /// </summary>
    /// <param name="targetSize">The target grid size to expand to.</param>
    private void ExpandSpiralTo(int targetSize)
    {
        // Build spiral incrementally, ring by ring
        // Start with 1×1 if needed
        if (_currentSpiralSize == 0)
        {
            _expandingSpiral.Add((0, 0));
            _spiralSet.Add((0, 0));
            _currentSpiralSize = 1;
        }
        
        // Expand ring by ring until we reach targetSize
        while (_currentSpiralSize < targetSize)
        {
            _currentSpiralSize++;
            AddRing(_currentSpiralSize);
        }
    }

    /// <summary>
    /// Adds a new ring to the expanding spiral grid pattern.
    /// </summary>
    /// <param name="size">The size of the ring to add.</param>
    private void AddRing(int size)
    {
        // Add the outer ring for a size×size grid
        // This adds new coordinates WITHOUT changing existing ones
        
        if (size == 1)
        {
            // Already added (0,0) in ExpandSpiralTo
            return;
        }
        
        // Helper method to add position only if not already in spiral
        void TryAddPosition(int row, int col)
        {
            var position = (row, col);
            if (_spiralSet.Add(position))
            {
                _expandingSpiral.Add(position);
            }
        }
        
        // Top row: add rightmost cell of top row
        TryAddPosition(0, size - 1);
        
        // Right column: go down from row 1 to size-1
        for (int row = 1; row < size; row++)
        {
            TryAddPosition(row, size - 1);
        }
        
        // Bottom row: go left from col size-2 to 0
        for (int col = size - 2; col >= 0; col--)
        {
            TryAddPosition(size - 1, col);
        }
        
        // Left column: go up from row size-2 to 1
        for (int row = size - 2; row >= 1; row--)
        {
            TryAddPosition(row, 0);
        }
    }

    public async Task<bool> ClearAllSquaresAsync()
    {
        await _repository.ClearAllAsync();
        _lastColor = null;
        
        // Reset spiral
        _expandingSpiral.Clear();
        _spiralSet.Clear();
        _currentSpiralSize = 0;
        
        return true;
    }
}
