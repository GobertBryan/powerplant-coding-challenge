using BusinessLogic.Interfaces;
using BusinessLogic.Queries;
using Microsoft.AspNetCore.Mvc;

namespace PowerPlantCodingChallenge.Controllers;

[Route("api/[controller]")]
public class EnergyController (IEnergyBL energyBL) : ControllerBase
{

    [HttpPost("productionplan")]
    public async Task<IActionResult> ProductionPlan([FromBody] PayLoad payLoad)
    {
        var result = await energyBL.GetPowerPlantsProduction(payLoad);

        return Ok(result);
    }
}