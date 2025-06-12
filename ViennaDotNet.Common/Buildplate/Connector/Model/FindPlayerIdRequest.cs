namespace ViennaDotNet.Common.Buildplate.Connector.Model;

public sealed record FindPlayerIdRequest(
    string minecraftId,
    string minecraftName
);