using System.Text.Json.Serialization;

namespace BusinessLogic.Queries;

public record PowerPlant(string Name, string Type, decimal Efficiency, [property: JsonPropertyName("pmin")] decimal MinimumProduction, [property: JsonPropertyName("pmax")] decimal MaximumProduction);
