using BusinessLogic.Queries;
using BusinessLogic.Results;

namespace BusinessLogic.Interfaces;

public interface IEnergyBL
{
    public Task<IEnumerable<PowerPlantInfo>> GetPowerPlantsProduction(PayLoad payload);
}