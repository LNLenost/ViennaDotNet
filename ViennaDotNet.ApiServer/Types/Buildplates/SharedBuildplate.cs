using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;
using System.Runtime.Serialization;
using ViennaDotNet.ApiServer.Types.Inventory;
using ViennaDotNet.DB.Models.Player;

namespace ViennaDotNet.ApiServer.Types.Buildplates;

public record SharedBuildplate(
    string playerId,
    string sharedOn,
    SharedBuildplate.BuildplateData buildplateData,
    Inventory.Inventory inventory
)
{
    public record BuildplateData(
        Dimension dimension,
        Offset offset,
        int blocksPerMeter,
        BuildplateData.Type type,
        SurfaceOrientation surfaceOrientation,
        String model,
        int order
    )
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Type
        {
            [EnumMember(Value = "Survival")] SURVIVAL,
        }
    }
}