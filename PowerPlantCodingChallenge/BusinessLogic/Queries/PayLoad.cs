using System.Text.Json.Serialization;

namespace BusinessLogic.Queries;

public record PayLoad([property : JsonPropertyName("load")] decimal Load,
    [property: JsonPropertyName("fuels")] Dictionary<string, decimal> Fuels,
    [property: JsonPropertyName("powerplants")] IEnumerable<PowerPlant> PowerPlants);