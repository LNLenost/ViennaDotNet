using System.Text.Json.Serialization;

namespace ViennaDotNet.ApiServer.Types.Buildplates;

public sealed record SharedBuildplate(
    string playerId,
    string sharedOn,
    SharedBuildplate.BuildplateData buildplateData,
    Inventory.Inventory inventory
)
{
    public sealed record BuildplateData(
        Dimension dimension,
        Offset offset,
        int blocksPerMeter,
        BuildplateData.Type type,
        SurfaceOrientation surfaceOrientation,
        string model,
        int order
    )
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Type
        {
            [JsonStringEnumMemberName("Survival")] SURVIVAL,
        }
    }
}