using BusinessLogic.Interfaces;
using BusinessLogic.Results;

namespace BusinessLogic;

public class EnergyBL : IEnergyBL
{
    public async Task<PowerPlantProduction> GetPowerPlantsProduction()
    {
        return new PowerPlantProduction([new PowerPlantInfo("gasfiredbig1", 5.14M)]);
    }
}