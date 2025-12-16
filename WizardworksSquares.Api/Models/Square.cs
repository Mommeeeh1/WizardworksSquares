namespace WizardworksSquares.Api.Models;

/// <summary>
/// Represents a square entity in the grid system.
/// </summary>
public class Square
{
    /// <summary>
    /// Unique identifier for the square.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Zero-based row index of the square in the grid.
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Zero-based column index of the square in the grid.
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Hexadecimal color code representing the square color (e.g. "#FF5733").
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp indicating when the square was created, in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
