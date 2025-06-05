using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ViennaDotNet.Buildplate.Connector.Model;

[JsonConverter(typeof(StringEnumConverter))]
public enum InventoryType
{
    SYNCED,
    DISCARD,
    BACKPACK
}