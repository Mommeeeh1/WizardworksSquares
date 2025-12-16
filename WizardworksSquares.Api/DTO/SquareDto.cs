namespace WizardworksSquares.Api.DTO;


/// <summary>
/// Data transfer object representing a square for API responses.
/// Used to transfer square data between the service layer and API endpoints.
/// </summary>

public record SquareDto(Guid Id, int Row, int Column, string Color, DateTime CreatedAt);
