namespace BusinessLogic.Enums;

public record PowerPlants_Fuels(int Id, string Fuel, string PowerplantType);


public static class PowerPlants_Fuels_List
{
    public static PowerPlants_Fuels Gas => new PowerPlants_Fuels(1, "gas(euro/MWh)", "gasfired");
    public static PowerPlants_Fuels Kerosene => new PowerPlants_Fuels(1, "kerosine(euro/MWh)", "turbojet");

    public static IEnumerable<PowerPlants_Fuels> GetAll()
    {
        return
        [
            Gas,
            Kerosene
        ];
    }
}