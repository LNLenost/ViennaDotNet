using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViennaDotNet.StaticData;

public sealed class TileRenderer
{
    public TileRenderer(string dir)
    {
        TagMapJson = File.ReadAllText(Path.Combine(dir, "tagMap.json"));
    }

    public string TagMapJson { get; }
}
