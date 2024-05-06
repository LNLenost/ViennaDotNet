using Serilog;
using System.Diagnostics;

namespace ViennaDotNet.Launcher.Programs
{
    internal static class ObjectStoreServer
    {
        const string dirName = "ObjectStoreServer";
        const string exeName = "ObjectStoreServer.exe";
        const string dispName = "ObjectStore server";

        public static bool Check()
        {
            string exePath = Path.GetFullPath(Path.Combine(dirName, exeName));
            if (!File.Exists(exePath))
            {
                Log.Error($"{dispName} exe doesn't exits: {exePath}");
                return false;
            }

            return true;
        }

        public static void Run(Settings settings)
        {
            Log.Information($"Running {dispName}");
            Process.Start(new ProcessStartInfo(Path.GetFullPath(Path.Combine(dirName, exeName)), new string[]
            {
                $"--dataDir={settings.ObjectStoreDataDir}",
                $"--port={settings.ObjectStorePort}"
            })
            {
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory, dirName),
                CreateNoWindow = false,
                UseShellExecute = true
            });
        }
    }
}
