using System.Text.Json.Serialization;
using ViennaDotNet.ApiServer.Types.Common;
using static ViennaDotNet.ApiServer.Types.Buildplates.BuildplateInstance;

namespace ViennaDotNet.ApiServer.Types.Buildplates;

public sealed record BuildplateInstance(
    string instanceId,
    string partitionId,
    string fqdn,
    string ipV4Address,
    int port,
    bool serverReady,
    ApplicationStatus applicationStatus,
    ServerStatus serverStatus,
    string metadata,
    GameplayMetadata gameplayMetadata,
    string roleInstance,    // TODO: find out what this is
    Coordinate hostCoordinate
)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApplicationStatus
    {
        [JsonStringEnumMemberName("Unknown")] UNKNOWN,
        [JsonStringEnumMemberName("Ready")] READY
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ServerStatus
    {
        [JsonStringEnumMemberName("Running")] RUNNING
    }

    public sealed record GameplayMetadata(
        string worldId,
        string templateId,
        string? spawningPlayerId,
        string spawningClientBuildNumber,
        string playerJoinCode,
        Dimension dimension,
        Offset offset,
        int blocksPerMeter,
        bool isFullSize,
        GameplayMetadata.GameplayMode gameplayMode,
        SurfaceOrientation surfaceOrientation,
        string? augmentedImageSetId,
        Rarity? rarity,
        Dictionary<string, object> breakableItemToItemLootMap    // TODO: find out what this is
    )
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum GameplayMode
        {
            [JsonStringEnumMemberName("Buildplate")] BUILDPLATE,
            [JsonStringEnumMemberName("BuildplatePlay")] BUILDPLATE_PLAY,
            [JsonStringEnumMemberName("SharedBuildplatePlay")] SHARED_BUILDPLATE_PLAY,
            [JsonStringEnumMemberName("Encounter")] ENCOUNTER
        }
    }
}
