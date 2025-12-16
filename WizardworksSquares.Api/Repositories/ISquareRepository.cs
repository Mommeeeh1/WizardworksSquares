using System;
using WizardworksSquares.Api.Models;

namespace WizardworksSquares.Api.Repositories;

/// <summary>
/// Repository interface for managing square data persistence.
/// Provides data access operations for squares stored in JSON format.
/// </summary>
public interface ISquareRepository
{
    /// <summary>
    /// Retrieves all squares from the data store.
    /// </summary>
    /// <returns>A list of all squares. Returns an empty list if no squares exist or if an error occurs.</returns>
    Task<List<Square>> GetAllAsync();

    /// <summary>
    /// Creates and persists a new square to the data store.
    /// </summary>
    /// <param name="square">The square entity to be created and saved.</param>
    /// <returns>The created square with its assigned ID and timestamp.</returns>
    Task<Square> CreateAsync(Square square);

    /// <summary>
    /// Removes all squares from the data store.
    /// </summary>
    Task ClearAllAsync();
}
