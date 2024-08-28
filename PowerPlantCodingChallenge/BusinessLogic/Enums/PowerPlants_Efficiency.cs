namespace BusinessLogic.Enums;

public record PowerPlants_Efficiency(int Id, string Efficiency, string PowerplantType);


public static class PowerPlantsEfficiency_List
{
    public static PowerPlants_Efficiency Wind => new (1, "wind(%)", "windturbine");

    public static IEnumerable<PowerPlants_Efficiency> GetAll()
    {
        return
        [
            Wind
        ];
    }
}