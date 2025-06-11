using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViennaDotNet.Common.Buildplate.Connector.Model;

public sealed record FindPlayerIdRequest(
    string minecraftId,
    string minecraftName
);