using BusinessLogic;
using BusinessLogic.Queries;
using BusinessLogic.Results;
using System.Text.Json;

namespace UnitTest;

public class EnergyBlTests
{
    [Theory]
    [InlineData("payload1", "response1")]
    [InlineData("payload2", "response2")]
    [InlineData("payload3", "response3")]
    public async Task GivenJsonPayloadInput_WhenGetPowerPlantsProduction_ThenReturnsJsonResponseWithGoodValues(string inputJsonName, string outputJsonName)
    {
        var inputPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, $"Inputs\\{inputJsonName}.json");
        var inputText = File.ReadAllText(inputPath);
        var inputJson = JsonSerializer.Deserialize<PayLoad>(inputText);

        var expectedPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, $"Results\\{outputJsonName}.json");
        var expectedText = File.ReadAllText(expectedPath);
        var expectedJson =  JsonSerializer.Deserialize<IEnumerable<PowerPlantInfo>>(expectedText)!.ToList();

        var bl = new EnergyBL();

        //  Act
        var result = bl.GetPowerPlantsProduction(inputJson!).ToList();

        //  Assert
        Assert.Equal(6, result.Count);
        
        for (var i = 0; i < result.Count; i++)
        {
            Assert.Equal(expectedJson[i].Name, result[i].Name);
            Assert.Equal(expectedJson[i].Value, result[i].Value);
        }
    }
}