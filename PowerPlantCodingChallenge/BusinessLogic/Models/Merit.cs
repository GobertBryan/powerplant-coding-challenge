using BusinessLogic.Queries;

namespace BusinessLogic.Models;

public record Merit(PowerPlant powerPlant, int order, decimal price);