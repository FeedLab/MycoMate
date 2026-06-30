using System.ComponentModel.DataAnnotations;

namespace MycoMate.Api.Models;

public class Mineral
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [MaxLength(100)] public required string Name { get; set; }
    [MaxLength(10)]  public required string ShortName { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    [MaxLength(30)]  public required string Unit { get; set; }
}
