namespace MycoMate.Maui.Models;

public record NutrientEntry(
    Guid NutrientId,
    string Name,
    string ShortName,
    string? Description,
    decimal Value,
    string Unit
);
