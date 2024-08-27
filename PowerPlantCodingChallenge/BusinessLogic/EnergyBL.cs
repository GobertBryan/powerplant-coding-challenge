using BusinessLogic.Enums;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using BusinessLogic.Queries;
using BusinessLogic.Results;

namespace BusinessLogic;

public class EnergyBL : IEnergyBL
{
    public async Task<PowerPlantProduction> GetPowerPlantsProduction(PayLoad payload)
    {
        var meritOrder = GetMeritOrder(payload);

        var powerPlantProduction = GetPowerPlantProduction(meritOrder, payload.Load);

        return powerPlantProduction;
    }

    private MeritOrder GetMeritOrder(PayLoad payload)
    {
        var meritOrder = new List<Merit>();

        var powerPlantsWithFuelPrice = GetFuelPriceForPowerPlants(payload);

        var powerPlantsWithFuelPriceOrdered = powerPlantsWithFuelPrice.OrderBy(x => x.price).ThenBy(x => x.powerPlant.MinimumProduction).ThenBy(x => x.powerPlant.MaximumProduction).ToList();

        for (var i = 0; i < powerPlantsWithFuelPriceOrdered.Count; i++)
        {
            meritOrder.Add(new Merit(powerPlantsWithFuelPriceOrdered[i].powerPlant, i + 1, powerPlantsWithFuelPriceOrdered[i].price));
        }

        return new MeritOrder(meritOrder);
    }

    private PowerPlantProduction GetPowerPlantProduction(MeritOrder meritOrder, decimal payloadToAchieve)
    {
        
    }

    private IEnumerable<(PowerPlant powerPlant, decimal price)> GetFuelPriceForPowerPlants(PayLoad payload)
    {
        var powerPlants_Fuels = PowerPlants_Fuels_List.GetAll();
        var powerPlants = payload.PowerPlants;
        var fuels = payload.Fuels;
        var meritOrder = new List<(PowerPlant powerPlant, decimal price)>();

        foreach (var powerPlant in powerPlants)
        {
            var linkBetweenPowerPlantTypeAndFuel = powerPlants_Fuels.SingleOrDefault(x => x.PowerplantType == powerPlant.Type);

            if (linkBetweenPowerPlantTypeAndFuel == null)
            {
                meritOrder.Add((powerPlant, 0));
            }
            else
            {
                var fuelPrice = fuels.Single(x => x.Key == linkBetweenPowerPlantTypeAndFuel.Fuel).Value;
                var efficiency = powerPlant.Efficiency;
                var cost = fuelPrice / efficiency;

                meritOrder.Add((powerPlant, cost));
            }
        }

        return meritOrder;
    }
}