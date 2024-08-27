using System.Text.Json.Serialization;

namespace BusinessLogic.Results;

public record PowerPlantInfo([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("p")] decimal Value);