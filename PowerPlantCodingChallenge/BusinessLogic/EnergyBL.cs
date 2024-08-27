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

        var powerPlantProduction = GetPowerPlantProduction(meritOrder, payload.Fuels, payload.Load);

        return powerPlantProduction;
    }

    private MeritOrder GetMeritOrder(PayLoad payload)
    {
        var meritOrder = new List<Merit>();

        var powerPlantsWithFuelPrice = GetFuelPriceForPowerPlants(payload);

        var powerPlantsWithFuelPriceOrdered = powerPlantsWithFuelPrice.OrderBy(x => x.price).ThenBy(x => x.powerPlant.MinimumProduction).ThenByDescending(x => x.powerPlant.MaximumProduction).ToList();

        for (var i = 0; i < powerPlantsWithFuelPriceOrdered.Count; i++)
        {
            meritOrder.Add(new Merit(powerPlantsWithFuelPriceOrdered[i].powerPlant, i + 1, powerPlantsWithFuelPriceOrdered[i].price));
        }

        return new MeritOrder(meritOrder);
    }

    private PowerPlantProduction GetPowerPlantProduction(MeritOrder meritOrder, Dictionary<string, decimal> fuels, decimal payloadToAchieve)
    {
        var efficiencyList = PowerPlantsEfficiencyList.GetAll();

        var result = new List<PowerPlantInfo>();

        foreach (var merit in meritOrder.Merits.OrderBy(x => x.order))
        {
            var powerPlant = merit.powerPlant;

            if (payloadToAchieve == 0)
            {
                result.Add(new PowerPlantInfo(powerPlant.Name, 0));

                continue;
            }

            var efficiency = efficiencyList.SingleOrDefault(x => x.PowerplantType == powerPlant.Type)?.Efficiency;

            var unitValue = efficiency is not null
                ? fuels.Single(x => x.Key == efficiency).Value /100
                : 1;

            var minProductionValue = powerPlant.MinimumProduction * unitValue;
            var maxProductionValue = powerPlant.MaximumProduction * unitValue;

            if (payloadToAchieve >= maxProductionValue)
            {
                result.Add(new PowerPlantInfo(powerPlant.Name, maxProductionValue));
                payloadToAchieve -= maxProductionValue;

            }
            else if (payloadToAchieve >= minProductionValue && payloadToAchieve < maxProductionValue)
            {
                result.Add(new PowerPlantInfo(powerPlant.Name, payloadToAchieve));
                payloadToAchieve = 0;
            }
            else
            {
                var position = merit.order;

                var previousMerit = meritOrder.Merits.Single(x => x.order == position);
                var efficiencyPreviousMerit = efficiencyList.SingleOrDefault(x => x.PowerplantType == powerPlant.Type)?.Efficiency;
                var unitValuePreviousMerit = efficiency is not null
                    ? fuels.Single(x => x.Key == efficiency).Value / 100
                    : 1;

                var maxProductionValuePreviousMerit = previousMerit.powerPlant.MaximumProduction * unitValuePreviousMerit;
                payloadToAchieve += maxProductionValuePreviousMerit;

                payloadToAchieve -= minProductionValue;
                result.Add(new PowerPlantInfo(previousMerit.powerPlant.Name, payloadToAchieve));
                result.Add(new PowerPlantInfo(powerPlant.Name, minProductionValue));

                payloadToAchieve = 0;
            }
        }

        return new PowerPlantProduction(result);
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