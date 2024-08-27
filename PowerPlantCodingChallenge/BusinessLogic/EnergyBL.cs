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

        var powerPlantsWithFuelCost = GetFuelCostByPowerPlant(payload);

        var powerPlantsWithFuelPriceOrdered = powerPlantsWithFuelCost.OrderBy(x => x.price).ThenBy(x => x.powerPlant.MinimumProduction).ThenByDescending(x => x.powerPlant.MaximumProduction).ToList();

        for (var i = 0; i < powerPlantsWithFuelPriceOrdered.Count; i++)
        {
            meritOrder.Add(new Merit(powerPlantsWithFuelPriceOrdered[i].powerPlant, i + 1, powerPlantsWithFuelPriceOrdered[i].price));
        }

        return new MeritOrder(meritOrder);
    }

    private PowerPlantProduction GetPowerPlantProduction(MeritOrder meritOrder, Dictionary<string, decimal> fuels, decimal payloadToAchieve)
    {
        var result = new List<PowerPlantInfo>();

        foreach (var merit in meritOrder.Merits.OrderBy(x => x.order))
        {
            var powerPlant = merit.powerPlant;

            //  if the objectif is reached we weep looping to add power plants with 0 produciton
            if (payloadToAchieve == 0)
            {
                result.Add(new PowerPlantInfo(powerPlant.Name, 0));

                continue;
            }

            var efficiency = GetEfficiency(powerPlant.Type);
            var unitValue = GetUnitValue(efficiency, fuels);
            var minProductionValue = powerPlant.MinimumProduction * unitValue;
            var maxProductionValue = powerPlant.MaximumProduction * unitValue;

            //  3 cases
            //  1) The left payload is bigger or equal than the pmax of the actual production plan, we can take all the power from the powerplant
            //  2) The left payload is between the pmin and the pmax of the actual production plan, we take what we need which is the value of the left payload
            //  3) We are in, a more difficult cas where the pmin is bigger than the left payload
            //     We need to take the pmin for the last powerplant
            //     and for the previous one, on take what is needed to provide the exact amount for the load
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
                var previousPosition = merit.order - 1;

                var previousMerit = meritOrder.Merits.Single(x => x.order == previousPosition);
                var efficiencyPreviousMerit = GetEfficiency(previousMerit.powerPlant.Type);
                var unitValuePreviousMerit = GetUnitValue(efficiencyPreviousMerit, fuels);

                var maxProductionValuePreviousMerit = previousMerit.powerPlant.MaximumProduction * unitValuePreviousMerit;
                payloadToAchieve += maxProductionValuePreviousMerit;
                payloadToAchieve -= minProductionValue;

                result.RemoveAt(result.Count - 1);
                result.Add(new PowerPlantInfo(previousMerit.powerPlant.Name, payloadToAchieve));
                result.Add(new PowerPlantInfo(powerPlant.Name, minProductionValue));

                payloadToAchieve = 0;
            }
        }

        return new PowerPlantProduction(result);
    }

    private IEnumerable<(PowerPlant powerPlant, decimal price)> GetFuelCostByPowerPlant(PayLoad payload)
    {
        var powerPlants_Fuels = PowerPlant_FuelPrice_List.GetAll();
        var powerPlants = payload.PowerPlants;
        var fuels = payload.Fuels;
        var fuelCostByPowerPlant = new List<(PowerPlant powerPlant, decimal price)>();

        foreach (var powerPlant in powerPlants)
        {
            var linkBetweenPowerPlantTypeAndFuel = powerPlants_Fuels.SingleOrDefault(x => x.PowerplantType == powerPlant.Type);

            if (linkBetweenPowerPlantTypeAndFuel == null)
            {
                fuelCostByPowerPlant.Add((powerPlant, 0));
            }
            else
            {
                var fuelPrice = fuels.Single(x => x.Key == linkBetweenPowerPlantTypeAndFuel.Fuel).Value;
                var efficiency = powerPlant.Efficiency;
                var cost = fuelPrice / efficiency;

                fuelCostByPowerPlant.Add((powerPlant, cost));
            }
        }

        return fuelCostByPowerPlant;
    }

    private string? GetEfficiency(string type) 
        => PowerPlantsEfficiency_List.GetAll().SingleOrDefault(x => x.PowerplantType == type)?.Efficiency;

    private decimal GetUnitValue(string? efficiency, Dictionary<string, decimal> fuels) 
        => efficiency is not null
                ? fuels.Single(x => x.Key == efficiency).Value / 100
                : 1;
}