using Serilog;

namespace ViennaDotNet.Launcher.Programs;

internal static class Fountain
{
    public const string DirName = "FountainBridge";

    public static string? JarName { get; private set; }

    public static bool Check()
    {
        if (check()) return true;

        Console.WriteLine("Fountain jar wasn't found");
        Console.WriteLine("Follow the *Building* instructions in https://github.com/Project-Genoa/Fountain-bridge#Building");
        Console.WriteLine($"Copy fountain-<version>-jar-with-dependencies.jar from [Fountain-bridge repo folder]/target to {DirName}");
        U.ConfirmType("done");

        if (check()) return true;
        else
        {
            Log.Error($"Fountain bridge jar doesn't exits");
            return false;
        }
    }

    private static bool check()
    {
        Directory.CreateDirectory(DirName);

        string? jarName = Directory.EnumerateFiles(DirName)
            .Select(file => Path.GetFileName(file))
            .Where(
                name => name.StartsWith("fountain") &&
                name.Contains("jar-with-dependencies") && /*make sure this is the bridge and not the mod*/
                Path.GetExtension(name) == ".jar"
            ).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(jarName))
            return false;
        else
        {
            JarName = jarName;
            return true;
        }
    }
}
