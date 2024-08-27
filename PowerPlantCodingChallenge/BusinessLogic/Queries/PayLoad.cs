namespace BusinessLogic.Queries;

public record PayLoad(decimal Load, string Type, Dictionary<string, decimal> Fuels, IEnumerable<PowerPlant> PowerPlants);