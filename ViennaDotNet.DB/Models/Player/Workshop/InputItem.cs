using ViennaDotNet.DB.Models.Common;

namespace ViennaDotNet.DB.Models.Player.Workshop;

public sealed record InputItem(
     string id,
     int count,
     NonStackableItemInstance[] instances
);
