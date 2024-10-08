﻿using BusinessLogic.Enums;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using BusinessLogic.Queries;
using BusinessLogic.Results;

namespace BusinessLogic;

public class EnergyBL : IEnergyBL
{
    public IEnumerable<PowerPlantInfo> GetPowerPlantsProduction(PayLoad payload)
    {
        var meritOrder = GetMeritOrder(payload);

        var powerPlantProduction = GetPowerPlantProduction(meritOrder, payload.Fuels, payload.Load);

        return powerPlantProduction;
    }

    private MeritOrder GetMeritOrder(PayLoad payload)
    {
        var meritOrder = new List<Merit>();

        var powerPlantsWithFuelCost = GetFuelCostByPowerPlant(payload);

        var powerPlantsWithFuelPriceOrdered = powerPlantsWithFuelCost.OrderBy(x => x.price).ThenByDescending(x => x.powerPlant.MinimumProduction).ThenByDescending(x => x.powerPlant.MaximumProduction).ToList();

        for (var i = 0; i < powerPlantsWithFuelPriceOrdered.Count; i++)
        {
            meritOrder.Add(new Merit(powerPlantsWithFuelPriceOrdered[i].powerPlant, i + 1, powerPlantsWithFuelPriceOrdered[i].price));
        }

        return new MeritOrder(meritOrder);
    }

    private IEnumerable<PowerPlantInfo> GetPowerPlantProduction(MeritOrder meritOrder, Dictionary<string, decimal> fuels, decimal payloadToAchieve)
    {
        var result = new List<PowerPlantInfo>();

        foreach (var merit in meritOrder.Merits.OrderBy(x => x.order))
        {
            var powerPlant = merit.powerPlant;

            //  If the objectif is reached we keep looping to add power plants with 0 produciton
            if (payloadToAchieve == 0)
            {
                AddPowerPlantInfo(result, new PowerPlantInfo(powerPlant.Name, 0M));

                continue;
            }

            var efficiency = GetEfficiency(powerPlant.Type);
            var unitValue = GetUnitValue(efficiency, fuels);

            var minProduction = GetMinProduction(powerPlant);
            var minProductionValue = minProduction * unitValue;
            var maxProductionValue = powerPlant.MaximumProduction * unitValue;

            //  3 cases
            //  1) The left payload is bigger or equal than the pmax of the actual production plan, we can take all the power from the powerplant
            //  2) The left payload is between the pmin and the pmax of the actual production plan, we take what we need which is the value of the left payload
            //  3) We are in, a more difficult cas where the pmin is bigger than the left payload
            //     More details on the third point

            //  1
            if (payloadToAchieve >= maxProductionValue)
            {
                AddPowerPlantInfo(result, new PowerPlantInfo(powerPlant.Name, maxProductionValue));
                payloadToAchieve -= maxProductionValue;

            }

            //  2
            else if (payloadToAchieve >= minProductionValue && payloadToAchieve < maxProductionValue)
            {
                AddPowerPlantInfo(result, new PowerPlantInfo(powerPlant.Name, payloadToAchieve));
                payloadToAchieve = 0;
            }

            //  3) This part is subdivised in 3 points (a,b and c)
            else
            {
                //  a) If there is no powerplant with a value yet in the answer and given the fact thant we are in the third case where pmin is bigger than the payloadToAchieve
                //     then, we can't use this power plant
                var lastPowerPlantInfoValued = result.Where(x => x.Value != 0).LastOrDefault();
                if (lastPowerPlantInfoValued is null)
                {
                    AddPowerPlantInfo(result, new PowerPlantInfo(powerPlant.Name, 0M));

                    continue;
                }

                //  b) If we are in the case where the sum of the actual pmin and the pmin from the last valued power plan in the result is bigger than the payloadToAchieve
                //     then, we add the actual power plant with the value 0
                //     IMPORTANT : Given the fact that when we put a value in the result for the last valued power, we removed this value from the payloadToAchieve,
                //                 we need to add this value tho the actual value of payloadToAchieve to compare with the sum of the actual pmin and the pmin from the last valued power plan
                var lastPowerPlantMeritValued = meritOrder.Merits.Single(x => x.powerPlant.Name == lastPowerPlantInfoValued?.Name).powerPlant;
                var lastMinProductionValued = GetMinProduction(lastPowerPlantMeritValued);

                if (lastMinProductionValued + minProduction > payloadToAchieve + lastPowerPlantInfoValued.Value)
                {
                    AddPowerPlantInfo(result, new PowerPlantInfo(powerPlant.Name, 0M));

                    continue;
                }

                //  c) In the last possible case, we need to use the pmin of the actual power plant for the actual power plant
                //     and modify the value of the last valued power plan in the result to be the value of the payloadToAchieve
                else
                {
                    var previousPowerPlantResult = result.Single(x => x.Name == lastPowerPlantMeritValued!.Name);

                    payloadToAchieve += previousPowerPlantResult.Value;
                    payloadToAchieve -= minProductionValue;

                    previousPowerPlantResult.UpdateValue(payloadToAchieve);
                    AddPowerPlantInfo(result, new PowerPlantInfo(powerPlant.Name, minProductionValue));

                    payloadToAchieve = 0;
                }           
            }
        }

        return result;
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

    private decimal GetMinProduction(PowerPlant powerPlant) => powerPlant.Type == "windturbine" ? powerPlant.MaximumProduction : powerPlant.MinimumProduction;

    private void AddPowerPlantInfo(List<PowerPlantInfo> list, PowerPlantInfo powerPlantInfo) => list.Add(new PowerPlantInfo(powerPlantInfo.Name, Math.Round(powerPlantInfo.Value, 1)));
}