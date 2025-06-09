namespace ViennaDotNet.StaticData;

public sealed class StaticData
{
    public readonly Catalog catalog;

    public StaticData(string dir)
    {
        catalog = new Catalog(Path.Combine(dir, "catalog"));
    }
}
