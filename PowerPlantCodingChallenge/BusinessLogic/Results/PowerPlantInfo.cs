using System.Text.Json.Serialization;

namespace BusinessLogic.Results;

public record PowerPlantInfo
{
    public PowerPlantInfo(string name, decimal value)
    {
        Name = name; 
        Value = value;
    }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("p")]
    public decimal Value { get; set; }
}