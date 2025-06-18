using Serilog;

namespace ViennaDotNet.Launcher.Programs;

internal static class MCServer
{
    public const string DirName = "MCServer";
    public const string ServerJarName = "fabric-server.jar";

    public static bool Check()
    {
        Status status;
        if ((status = check()) == Status.OK) return true;

        if (status.HasFlag(Status.ServerMissing))
        {
            Console.WriteLine("Server file wasn't found");
            Console.WriteLine("Go to https://fabricmc.net/use/server/, select minecraft version 1.20.4 and download the jar");
            Console.WriteLine($"Move it to {DirName} and rename it to {ServerJarName}");
            Console.WriteLine($"Run it once and close it");
            U.ConfirmType("done");
        }

        if (status.HasFlag(Status.FabricApiMissing))
        {
            Console.WriteLine("Fabric api file wasn't found");
            Console.WriteLine("Go to https://modrinth.com/mod/fabric-api/versions?g=1.20.4&c=release, and download the latest version");
            Console.WriteLine($"Move it to {DirName}/mods");
            U.ConfirmType("done");
        }

        if (status.HasFlag(Status.FountainModMissing))
        {
            Console.WriteLine("Fountain mod wasn't found");
            Console.WriteLine("If you don't have it yet, download and \"install\" maven");
            Console.WriteLine("Follow the *Building* instructions in https://github.com/Project-Genoa/Fountain-fabric?tab=readme-ov-file#Building");
            Console.WriteLine($"Copy the fountain-<version>.jar file from [Fountain-fabric repo folder]/build/libs/ to {DirName}/mods");
            U.ConfirmType("done");
        }

        if ((status = check()) == Status.OK) return true;
        else
        {
            if (status.HasFlag(Status.ServerMissing)) Log.Error($"{DirName}/{ServerJarName} missing");
            if (status.HasFlag(Status.FabricApiMissing)) Log.Error($"Fabric api missing");
            if (status.HasFlag(Status.FountainModMissing)) Log.Error($"Fountain mod missing");

            return false;
        }
    }
    private static Status check()
    {
        Directory.CreateDirectory(DirName);

        Status status = Status.OK;

        if (!File.Exists(Path.Combine(DirName, ServerJarName)))
            status |= Status.ServerMissing;

        string modsDir = Path.Combine(DirName, "mods");
        Directory.CreateDirectory(modsDir);
        IEnumerable<string> files = Directory.EnumerateFiles(modsDir)
            .Select(file => Path.GetFileName(file));

        if (!files.Any(name => name.StartsWith("fabric-api") &&
            Path.GetExtension(name) == ".jar")
        )
        {
            status |= Status.FabricApiMissing;
        }

        if (!files.Any(name => name.StartsWith("fountain") &&
            Path.GetExtension(name) == ".jar" &&
            !name.Contains("SNAPSHOT")/*make sure this is NOT the bridge*/))
        {
            status |= Status.FountainModMissing;
        }

        return status;
    }

    [Flags]
    private enum Status : byte
    {
        OK = 0b_0000,
        ServerMissing = 0b_0001,
        FabricApiMissing = 0b_0010,
        FountainModMissing = 0b_0100,
    }
}
