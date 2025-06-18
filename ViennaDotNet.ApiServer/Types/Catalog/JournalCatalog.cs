using static ViennaDotNet.ApiServer.Types.Catalog.JournalCatalog;

namespace ViennaDotNet.ApiServer.Types.Catalog;

public sealed record JournalCatalog(
    Dictionary<string, Item> items
)
{
    public sealed record Item(
        string referenceId,
        string parentCollection,
        int overallOrder,
        int collectionOrder,
        string? defaultSound,
        bool deprecated,
        string toolsVersion
    );
}
