using BusinessLogic.Queries;
using BusinessLogic.Results;

namespace BusinessLogic.Interfaces;

public interface IEnergyBL
{
    public IEnumerable<PowerPlantInfo> GetPowerPlantsProduction(PayLoad payload);
}