using System;
using WizardworksSquares.Api.DTO;

namespace WizardworksSquares.Api.Services;

/// <summary>
/// Service interface for square business logic operations.
/// Handles square creation with automatic positioning, color assignment, and grid management.
/// </summary>
public interface ISquareService
{
    /// <summary>
    /// Retrieves all squares as data transfer objects.
    /// </summary>
    /// <returns>A list of all squares as DTOs.</returns>
    Task<List<SquareDto>> GetAllSquaresAsync();

    /// <summary>
    /// Creates a new square with automatically calculated position and random color.
    /// The position is determined by the grid algorithm to maintain a roughly square grid shape,
    /// filling left-to-right, top-to-bottom, with right-alignment for incomplete last rows.
    /// </summary>
    /// <returns>The created square as a DTO with assigned position, color, and timestamp.</returns>
    Task<SquareDto> CreateSquareAsync();

    /// <summary>
    /// Removes all squares from the system and resets the color selection state.
    /// </summary>
    /// <returns>True if the operation completed successfully.</returns>
    Task<bool> ClearAllSquaresAsync();
}
