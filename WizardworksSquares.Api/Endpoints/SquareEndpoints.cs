using WizardworksSquares.Api.DTO;
using WizardworksSquares.Api.Services;
using Microsoft.AspNetCore.Http;

namespace WizardworksSquares.Api.Endpoints;

/// <summary>
/// Provides REST endpoints for retrieving, creating, and clearing squares.
/// </summary>
public static class SquareEndpoints
{
    /// <summary>
    /// Maps all square-related endpoints to the application.
    /// </summary>
    public static void MapSquareEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/squares")
            .WithTags("Squares");

        group.MapGet("/", async (ISquareService squareService) =>
        {
            var squares = await squareService.GetAllSquaresAsync();
            return Results.Ok(squares);
        })
        .WithName("GetAllSquares")
        .WithSummary("Retrieves all squares from the system.")
        .WithDescription("Returns a list of all squares currently stored in the system. Each square includes its position (row, column), color, unique identifier, and creation timestamp.")
        .Produces<List<SquareDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (ISquareService squareService) =>
        {
            var square = await squareService.CreateSquareAsync();
            return Results.Created($"/api/squares/{square.Id}", square);
        })
        .WithName("CreateSquare")
        .WithSummary("Creates a new square with automatically calculated position and random color.")
        .WithDescription("Creates a new square in the grid system. The position is automatically calculated using a spiral algorithm to maintain a roughly square grid shape. The color is randomly selected from the available color palette. The square is persisted to storage and will be available on subsequent requests.")
        .Produces<SquareDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/", async (ISquareService squareService) =>
        {
            await squareService.ClearAllSquaresAsync();
            return Results.NoContent();
        })
        .WithName("ClearAllSquares")
        .WithSummary("Removes all squares from the system.")
        .WithDescription("Permanently removes all squares from the system and resets the grid. This operation cannot be undone. The color selection state is also reset, allowing colors to be reused from the beginning of the palette.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
