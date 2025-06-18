using ViennaDotNet.ApiServer.Types.Common;

namespace ViennaDotNet.ApiServer.Types.Catalog;

public sealed record NFCBoost(
    string id,
    string name,
    string type,
    Rewards rewards,
    BoostMetadata boostMetadata,
    bool deprecated,
    string toolsVersion
);
