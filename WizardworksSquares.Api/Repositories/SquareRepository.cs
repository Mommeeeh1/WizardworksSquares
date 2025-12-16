using System.Text.Json;
using WizardworksSquares.Api.Constants;
using WizardworksSquares.Api.Models;

namespace WizardworksSquares.Api.Repositories;


/// <summary>
/// JSON file-based implementation of the square repository.
/// Persists square data to a local JSON file.
/// </summary>
public class SquareRepository : ISquareRepository
{
    private readonly string _filePath;
    private readonly ILogger<SquareRepository> _logger;

    public SquareRepository(IConfiguration configuration, ILogger<SquareRepository> logger)
    {
        var dataPath = configuration["DataPath"] ?? "Data";
        _filePath = Path.Combine(dataPath, SquareConstants.SquaresFileName);
        _logger = logger;
        
        Directory.CreateDirectory(dataPath);

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public async Task<List<Square>> GetAllAsync()
    {
        try
        {
            // Handle empty or missing file
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            {
                _logger.LogWarning("Squares file is empty or missing, returning empty list");
                return new List<Square>();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            
            // Validate JSON is not empty or whitespace
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Squares file contains only whitespace, returning empty list");
                return new List<Square>();
            }

            var squares = JsonSerializer.Deserialize<List<Square>>(json);
            
            // Validate deserialized data
            if (squares == null)
            {
                _logger.LogWarning("Failed to deserialize squares, returning empty list");
                return new List<Square>();
            }

            // Filter out invalid squares
            var validSquares = squares.Where(s => s != null && s.Id != Guid.Empty).ToList();
            
            if (validSquares.Count != squares.Count)
            {
                _logger.LogWarning("Filtered out {Count} invalid squares", squares.Count - validSquares.Count);
            }

            return validSquares;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error in squares file. File may be corrupted.");
            return new List<Square>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error reading squares from JSON file");
            return new List<Square>();
        }
    }

    public async Task<Square> CreateAsync(Square square)
    {
        // Validate input
        if (square == null)
        {
            throw new ArgumentNullException(nameof(square), "Square cannot be null");
        }

        if (square.Id == Guid.Empty)
        {
            throw new ArgumentException("Square must have a valid Id", nameof(square));
        }

        var squares = await GetAllAsync();
        squares.Add(square);
        
        var json = JsonSerializer.Serialize(squares, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        await File.WriteAllTextAsync(_filePath, json);
        _logger.LogDebug("Successfully created square with Id: {Id}", square.Id);
        return square;
    }
    public async Task ClearAllAsync()
    {
        await File.WriteAllTextAsync(_filePath, "[]");
        _logger.LogInformation("All squares cleared");
    }
}
