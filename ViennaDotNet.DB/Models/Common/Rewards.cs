namespace ViennaDotNet.DB.Models.Common;

public sealed record Rewards(
    int rubies,
    int experiencePoints,
    int? level,
    Dictionary<string, int?> items,
    string[] buildplates,
    string[] challenges
);
