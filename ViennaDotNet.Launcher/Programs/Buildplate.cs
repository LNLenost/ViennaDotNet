using Serilog;
using System.Diagnostics;

namespace ViennaDotNet.Launcher.Programs
{
    internal static class Buildplate
    {
        const string dirName = "Buildplate";
        const string exeName = "BuildplateLauncher.exe";
        const string dispName = "Buildplate launcher";

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

        public static void Run(Settings settings, string bridgeJar, string serverTemplateDir, string fabricJarName, string connectorPluginJar)
        {
            Log.Information($"Running {dispName}");
            Process.Start(new ProcessStartInfo(Path.GetFullPath(Path.Combine(dirName, exeName)), new string[]
            {
                $"--eventbus=localhost:{settings.EventBusPort}",
                $"--publicAddress={settings.IPv4}",
                $"--bridgeJar={bridgeJar}",
                $"--serverTemplateDir={serverTemplateDir}",
                $"--fabricJarName={fabricJarName}",
                $"--connectorPluginJar={connectorPluginJar}"
            })
            {
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory, dirName),
                CreateNoWindow = false,
                UseShellExecute = true
            });
        }
    }
}
