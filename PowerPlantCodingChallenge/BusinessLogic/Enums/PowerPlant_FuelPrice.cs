namespace BusinessLogic.Enums;

public record PowerPlant_FuelPrice(int Id, string Fuel, string PowerplantType);

public static class PowerPlant_FuelPrice_List
{
    public static PowerPlant_FuelPrice Gas => new (1, "gas(euro/MWh)", "gasfired");
    public static PowerPlant_FuelPrice Kerosene => new (2, "kerosine(euro/MWh)", "turbojet");

    public static IEnumerable<PowerPlant_FuelPrice> GetAll()
    {
        return
        [
            Gas,
            Kerosene
        ];
    }
}