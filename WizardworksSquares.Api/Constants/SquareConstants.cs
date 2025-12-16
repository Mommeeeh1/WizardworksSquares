namespace WizardworksSquares.Api.Constants;

/// <summary>
/// Constants used throughout the squares application.
/// </summary>
public static class SquareConstants
{
    /// <summary>
    /// Available colors for squares in hexadecimal format.
    /// </summary>
    public static readonly string[] AvailableColors = new[]
    {
        "#FF5733", "#33FF57", "#3357FF", "#FF33F5", "#F5FF33", "#33FFF5",
        "#FF6B33", "#6B33FF", "#33FF6B", "#FF336B", "#6BFF33", "#336BFF"
    };

    /// <summary>
    /// Default filename for storing squares data.
    /// </summary>
    public const string SquaresFileName = "squares.json";
}
