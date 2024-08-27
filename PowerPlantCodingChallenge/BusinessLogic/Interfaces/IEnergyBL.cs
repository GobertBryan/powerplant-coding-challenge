using BusinessLogic.Results;

namespace BusinessLogic.Interfaces;

public interface IEnergyBL
{
    public Task<PowerPlantProduction> GetPowerPlantsProduction();
}