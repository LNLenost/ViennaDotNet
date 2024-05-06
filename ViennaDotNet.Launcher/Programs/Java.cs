using Serilog;
using ViennaDotNet.Common.Utils;

namespace ViennaDotNet.Launcher.Programs
{
    internal static class Java
    {
        public static void Check()
        {
            if (check()) return;

            Console.WriteLine("Java from JAVA_HOME is not suitable (does not exist or cannot be accessed)");
            Console.WriteLine("Download and install Java 17 from https://www.oracle.com/java/technologies/downloads/#jdk17-windows");
            U.ConfirmType("done");

            if (check()) return;

            Log.Warning("Java from JAVA_HOME is not suitable (does not exist or cannot be accessed)");
            Log.Information("\"java\" will be used for java server and bridge");
        }

        private static bool check()
        {
            string? javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(javaHome))
            {
                try
                {
                    FileInfo file = new FileInfo(Path.Combine(javaHome, "bin", "java"));
                    if (file.CanExecute())
                        return true;

                    file = new FileInfo(Path.Combine(javaHome, "bin", "java.exe"));
                    if (file.CanExecute())
                        return true;
                }
                catch
                {
                }
            }

            return false;
        }
    }
}
