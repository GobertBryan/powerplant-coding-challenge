using BusinessLogic.Queries;
using BusinessLogic.Results;

namespace BusinessLogic.Interfaces;

public interface IEnergyBL
{
    public Task<PowerPlantProduction> GetPowerPlantsProduction(PayLoad payload);
}