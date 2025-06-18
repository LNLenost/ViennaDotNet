using ViennaDotNet.ApiServer.Types.Common;

namespace ViennaDotNet.ApiServer.Types.Profile;

public sealed record Profile(
    Dictionary<int, Profile.Level> levelDistribution,
    int totalExperience,
    int level,
    int currentLevelExperience,
    int experienceRemaining,
    int health,
    float healthPercentage
)
{
    public sealed record Level(
        int experienceRequired,
        Rewards rewards
    );
}
