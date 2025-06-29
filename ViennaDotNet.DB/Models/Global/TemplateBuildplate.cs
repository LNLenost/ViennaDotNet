using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViennaDotNet.DB.Models.Global;

public sealed record TemplateBuildplate(
    int Size,
    int Offset,
    int Scale,
    bool Night,
    string ServerDataObjectId,
    string PreviewObjectId
);