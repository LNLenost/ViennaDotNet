using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ViennaDotNet.ApiServer.Utils;

namespace ViennaDotNet.ApiServer.Controllers;

[ApiVersion("1.1")]
[Route("cdn/tile/16/{_}/{tilePos1}_{tilePos2}_16.png")]
[ResponseCache(Duration = 11200)]
public class CdnTileController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTile(int _, int tilePos1, int tilePos2) // _ used because we dont care :|
    {
        string targetTilePath = $"./data/tiles/16/{tilePos1}/{tilePos1}_{tilePos2}_16.png";

        if (!System.IO.File.Exists(targetTilePath))
        {
            if (!await TileUtils.TryGetTile(tilePos1, tilePos2, @"./data/tiles/16/"))
            {
                return NotFound();
            }
        }

        var cd = new System.Net.Mime.ContentDisposition { FileName = tilePos1 + "_" + tilePos2 + "_16.png", Inline = true };
        Response.Headers.Append("Content-Disposition", cd.ToString());

        return File(System.IO.File.OpenRead(targetTilePath), "application/octet-stream", tilePos1 + "_" + tilePos2 + "_16.png");
    }
}
