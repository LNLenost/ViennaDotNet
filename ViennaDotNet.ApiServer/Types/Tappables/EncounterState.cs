using System.Text.Json.Serialization;

namespace ViennaDotNet.ApiServer.Types.Tappables;

public sealed record EncounterState(
    EncounterState.ActiveEncounterState activeEncounterState
)
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActiveEncounterState
    {
        [JsonStringEnumMemberName("Pristine")] PRISTINE,
        [JsonStringEnumMemberName("Dirty")] DIRTY,
    }
}