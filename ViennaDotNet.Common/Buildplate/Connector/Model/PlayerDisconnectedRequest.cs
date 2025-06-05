using ViennaDotNet.Common.Buildplate.Connector.Model;

namespace ViennaDotNet.Buildplate.Connector.Model;

public record PlayerDisconnectedRequest(
     string playerId,
     InventoryResponse? backpackContents
)
{
}
