using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace XybLauncher
{
    public class VersionData
    {
        public string Selected { get; set; }
        public VersionInfo[] Versions { get; set; }
    }

    public class VersionInfo
    {
        public string Season { get; set; }
        public string Path { get; set; }
    }

    public class StartHandler
    {
        public async Task StartFortnite(string jsonFilePath, string dllDownload, string dllName)
        {
            Console.WriteLine("Initializing Fortnite Launcher...");

            if (!File.Exists(jsonFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Version configuration file not found at {jsonFilePath}");
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var versionData = JsonConvert.DeserializeObject<VersionData>(jsonContent);
                var selectedVersion = versionData?.Versions.FirstOrDefault(v => v.Season == versionData.Selected);

                if (selectedVersion == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Selected version '{versionData?.Selected}' not found in the configuration.");
                    return;
                }

                string gamePath = selectedVersion.Path;
                string joinedArgs = "-log -nosound -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d";

                if (!File.Exists(Path.Combine(gamePath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: Fortnite executable missing in {gamePath}");
                    return;
                }

                Process fnProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(gamePath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe"),
                        Arguments = joinedArgs,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    }
                };

                fnProcess.Start();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Fortnite process started successfully.");

                // Async read standard output and error
                Task outputReader = Task.Run(() => ReadProcessOutput(fnProcess.StandardOutput, ConsoleColor.White));
                Task errorReader = Task.Run(() => ReadProcessOutput(fnProcess.StandardError, ConsoleColor.Red));

                // Wait for process exit
                await Task.WhenAll(outputReader, errorReader);
                fnProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred while starting Fortnite: {ex.Message}");
            }
        }

        private void ReadProcessOutput(StreamReader reader, ConsoleColor color)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(line);
            }
        }
    }
}
