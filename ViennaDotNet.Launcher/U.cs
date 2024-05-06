namespace ViennaDotNet.Launcher
{
    internal static class U
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exit">If <see langword="true"/> says 'Press any key to exit...'; otherwise says 'Press any key to continue...'</param>
        public static void PAK(bool exit = false)
        {
            Console.WriteLine($"Press any key to {(exit ? "exit" : "continue")}...");
            Console.ReadKey(true);
        }

        public static void ConfirmType(string textToType)
        {
            textToType = textToType.ToLowerInvariant();

            Console.WriteLine($"Type \"{textToType}\" (without \") and hit enter to proceed");

            while (true)
            {
                string typed = Console.ReadLine()?.ToLowerInvariant() ?? string.Empty;

                if (typed == textToType)
                    return;

                Console.WriteLine("Incorrect input. Try again.");
            }
        }
    }
}
