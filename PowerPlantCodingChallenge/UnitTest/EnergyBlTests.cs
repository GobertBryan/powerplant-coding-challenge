using BusinessLogic;
using BusinessLogic.Queries;
using BusinessLogic.Results;
using System.Text.Json;

namespace UnitTest;

public class EnergyBlTests
{
    [Fact]
    public async Task GivenJsonPayloadInput_WhenGetPowerPlantsProduction_ThenReturnsJsonResponseWithGoodValues()
    {
        var inputPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, "Inputs\\payload1.json");
        var inputText = File.ReadAllText(inputPath);
        var inputJson = JsonSerializer.Deserialize<PayLoad>(inputText);

        var expectedPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName, "Results\\response1.json");
        var expectedText = File.ReadAllText(expectedPath);
        var expectedJson =  JsonSerializer.Deserialize<IEnumerable<PowerPlantInfo>>(expectedText)!.ToList();

        var bl = new EnergyBL();

        //  Act
        var result = (await bl.GetPowerPlantsProduction(inputJson!)).ToList();

        //  Assert
        Assert.Equal(6, result.Count);
        
        for (var i = 0; i < result.Count; i++)
        {
            Assert.Equal(expectedJson[i].Name, result[i].Name);
            Assert.Equal(expectedJson[i].Value, result[i].Value);
        }
    }
}