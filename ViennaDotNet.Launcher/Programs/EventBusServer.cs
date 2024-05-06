using Serilog;
using System.Diagnostics;

namespace ViennaDotNet.Launcher.Programs
{
    internal static class EventBusServer
    {
        const string dirName = "EventBusServer";
        const string exeName = "EventBusServer.exe";
        const string dispName = "EventBus server";

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
                $"--port={settings.EventBusPort}"
            })
            {
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory, dirName),
                CreateNoWindow = false,
                UseShellExecute = true
            });
        }
    }
}
