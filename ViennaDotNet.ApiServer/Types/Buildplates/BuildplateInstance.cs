using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using ViennaDotNet.ApiServer.Types.Common;
using static ViennaDotNet.ApiServer.Types.Buildplates.BuildplateInstance;

namespace ViennaDotNet.ApiServer.Types.Buildplates;

public record BuildplateInstance(
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
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApplicationStatus
    {
        [EnumMember(Value = "Unknown")] UNKNOWN,
        [EnumMember(Value = "Ready")] READY
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ServerStatus
    {
        [EnumMember(Value = "Running")] RUNNING
    }

    public record GameplayMetadata(
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
        [JsonConverter(typeof(StringEnumConverter))]
        public enum GameplayMode
        {
            [EnumMember(Value = "Buildplate")] BUILDPLATE,
            [EnumMember(Value = "BuildplatePlay")] BUILDPLATE_PLAY,
            [EnumMember(Value = "SharedBuildplatePlay")] SHARED_BUILDPLATE_PLAY,
            [EnumMember(Value = "Encounter")] ENCOUNTER
        }
    }
}
