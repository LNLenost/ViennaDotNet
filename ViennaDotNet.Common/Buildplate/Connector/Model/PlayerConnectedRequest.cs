namespace ViennaDotNet.Buildplate.Connector.Model;

public sealed record PlayerConnectedRequest(
    string uuid,
    string joinCode
);