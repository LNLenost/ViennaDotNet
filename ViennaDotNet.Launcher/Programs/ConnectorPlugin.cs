using Serilog;

namespace ViennaDotNet.Launcher.Programs
{
    internal static class ConnectorPlugin
    {
        public const string JarName = "buildplate-connector-plugin-0.0.1-SNAPSHOT-jar-with-dependencies.jar";

        public static bool Check()
        {
            string path = Path.GetFullPath(JarName);

            if (!File.Exists(path))
            {
                Log.Error($"Buildplate connector plugin doesn't exits: {path}");
                return false;
            }

            return true;
        }
    }
}
