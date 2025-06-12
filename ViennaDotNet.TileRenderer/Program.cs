using Npgsql;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViennaDotNet.TileRenderer;

// only for testing, TODO: convert to lib type

var log = new LoggerConfiguration()
   .WriteTo.Console()
   .MinimumLevel.Debug()
   .CreateLogger();

Log.Logger = log;

string connectionString = args[0];
await using var dataSource = NpgsqlDataSource.Create(connectionString);

Renderer renderer = Renderer.Create(File.ReadAllText("tagMap.json"), log);

using (var bitmap = new SKBitmap(128, 128))
using (var canvas = new SKCanvas(bitmap))
{
    await renderer.RenderAsync(dataSource, canvas, 50.081604, 14.410044, 16);

    using (var data = bitmap.Encode(SKEncodedImageFormat.Png, 80))
    using (var stream = File.OpenWrite("tile.png"))
    {
        Log.Information("Writing png...");
        data.SaveTo(stream);
    }
}