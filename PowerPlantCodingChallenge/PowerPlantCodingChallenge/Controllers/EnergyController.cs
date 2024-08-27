using BusinessLogic.Interfaces;
using BusinessLogic.Queries;
using Microsoft.AspNetCore.Mvc;

namespace PowerPlantCodingChallenge.Controllers;

[Route("api/[controller]")]
public class EnergyController (IEnergyBL energyBL) : ControllerBase
{

    [HttpPost("productionplan")]
    public ActionResult ProductionPlan([FromBody] PayLoad payLoad)
    {
        var result = energyBL.GetPowerPlantsProduction(payLoad);

        return Ok(result);
    }
}