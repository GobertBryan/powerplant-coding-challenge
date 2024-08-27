using Microsoft.AspNetCore.Mvc;

namespace PowerPlantCodingChallenge.Controllers;

[Route("api/[controller]")]
public class EnergyController : ControllerBase
{
    [HttpPost("productionplan")]
    public decimal ProductionPlan([FromBody] PayLoad payLoad)
    {
        return payLoad.Load;
    }
}