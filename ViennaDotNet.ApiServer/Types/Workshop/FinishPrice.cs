namespace ViennaDotNet.ApiServer.Types.Workshop;

public sealed record FinishPrice(
    int cost,
    int discount,
    string validTime
);
