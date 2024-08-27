using System.Text.Json.Serialization;

namespace BusinessLogic.Results;

public record PowerPlantInfo(string Name, [property: JsonPropertyName("p")] decimal Value);