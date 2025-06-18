namespace ViennaDotNet.ApiServer.Types.Workshop;

public sealed record InputItem(
     string itemId,
     int quantity,
     string[] instanceIds
);