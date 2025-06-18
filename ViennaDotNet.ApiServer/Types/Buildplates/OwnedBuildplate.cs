using System.Text.Json.Serialization;

namespace ViennaDotNet.ApiServer.Types.Buildplates;

public sealed record OwnedBuildplate(
    string id,
    string templateId,
    Dimension dimension,
    Offset offset,
    int blocksPerMeter,
    OwnedBuildplate.Type type,
    SurfaceOrientation surfaceOrientation,
    string model,
    int order,
    bool locked,
    int requiredLevel,
    bool isModified,
    string lastUpdated,
    int numberOfBlocks,
    string eTag
)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Type
    {
        [JsonStringEnumMemberName("Survival")] SURVIVAL
    }
}
