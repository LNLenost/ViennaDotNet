using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ViennaDotNet.Common.Buildplate.Connector.Model;

public sealed record InitialPlayerStateResponse(
    float health,
    InitialPlayerStateResponse.BoostStatusEffect[] boostStatusEffects
)
{
    public sealed record BoostStatusEffect(
        BoostStatusEffect.Type type,
        int value,
        long remainingDuration
    )
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Type
        {
            ADVENTURE_XP,
            DEFENSE,
            EATING,
            HEALTH,
            MINING_SPEED,
            STRENGTH
        }
    }
}