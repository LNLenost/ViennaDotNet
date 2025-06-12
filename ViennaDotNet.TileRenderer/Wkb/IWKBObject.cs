using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViennaDotNet.TileRenderer.Wkb;

internal interface IWKBObject
{
    bool ByteOrder { get; }

    uint WkbType { get; }

    static virtual IWKBObject Load(BinaryReader reader)
        => throw new NotImplementedException();

    void Render(SKCanvas canvas, Tile tile, SKColor color, float strokeWidth);
}
