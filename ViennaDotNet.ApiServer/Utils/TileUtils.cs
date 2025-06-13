using ViennaDotNet.TileRenderer;

namespace ViennaDotNet.ApiServer.Utils;

internal static class TileUtils
{
    public static async Task<bool> TryGetTile(int pos1, int pos2, string basePath)
    {
        Directory.CreateDirectory(Path.Combine(basePath, pos1.ToString()));

        // get from renderer over event bus
    }

    //From https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames with slight changes
    private static (int X, int Y) getTileForCords(double lat, double lon, int zoom = 16)
    {
        int xtile = (int)double.Floor((lon + 180) / 360 * (1 << zoom));
        int ytile = (int)double.Floor((1 - double.Log(double.Tan(DegToRad(lat)) + 1 / double.Cos(DegToRad(lat))) / double.Pi) / 2 * (1 << zoom));

        if (xtile < 0)
        {
            xtile = 0;
        }

        if (xtile >= (1 << zoom))
        {
            xtile = (1 << zoom) - 1;
        }

        if (ytile < 0)
        {
            ytile = 0;
        }

        if (ytile >= (1 << zoom))
        {
            ytile = (1 << zoom) - 1;
        }

        return (xtile, ytile);
    }

    private static double DegToRad(double angle)
        => (double.Pi / 180d) * angle;
}
