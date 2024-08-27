using System.Text.Json.Serialization;

namespace BusinessLogic.Queries;

public record PowerPlant([property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("efficiency")] decimal Efficiency,
    [property: JsonPropertyName("pmin")] decimal MinimumProduction,
    [property: JsonPropertyName("pmax")] decimal MaximumProduction);
