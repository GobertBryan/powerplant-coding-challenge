namespace BusinessLogic.Enums;

public record PowerPlantsEfficiency(int Id, string Efficiency, string PowerplantType);


public static class PowerPlantsEfficiencyList
{
    public static PowerPlantsEfficiency Wind => new PowerPlantsEfficiency(1, "wind(%)", "windturbine");

    public static IEnumerable<PowerPlantsEfficiency> GetAll()
    {
        return
        [
            Wind
        ];
    }
}