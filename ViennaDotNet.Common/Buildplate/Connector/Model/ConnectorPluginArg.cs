namespace ViennaDotNet.Buildplate.Connector.Model;

public sealed record ConnectorPluginArg(
    string eventBusAddress,
    string eventBusQueueName,
    bool saveEnabled,
    InventoryType inventoryType
);
