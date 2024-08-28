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
    [InlineData("payload4", "response4")]
    [InlineData("payload5", "response5")]
    [InlineData("payload6", "response6")]
    [InlineData("payload7", "response7")]
    [InlineData("payload8", "response8")]
    [InlineData("payload9", "response9")]
    [InlineData("payload10", "response10")]
    [InlineData("payload11", "response11")]
    [InlineData("payload12", "response12")]
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