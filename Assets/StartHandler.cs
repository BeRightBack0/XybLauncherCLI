using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spectre.Console;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Terminal.Gui.Graphs.PathAnnotation;
using static XybLauncher.Program;
using System.Net.Http;
using System.Text.Json.Serialization;




namespace XybLauncher
{
    public class StartHandler
    {
       public string selectedPath;



        public string SelectedGamePath { get; private set; }

        public async Task StartFortnite(string jsonFilePath, string dllDownload, string dllName)
        {
            Console.WriteLine("Starting Fortnite...");

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"Error: File does not exist at path: {jsonFilePath}");
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var versionData = JsonConvert.DeserializeObject<VersionData>(jsonContent);
                Console.WriteLine($"Selected version: {versionData.Selected}");

                var selectedVersion = versionData.Versions.FirstOrDefault(v => v.Season == versionData.Selected);

                if (selectedVersion != null)
                {
                    SelectedGamePath = selectedVersion.Path;
                    Console.WriteLine($"Selected game path: {SelectedGamePath}");
                    await StartGame(SelectedGamePath, dllDownload, dllName);
                }
                else
                {
                    Console.WriteLine($"Error: Selected version '{versionData.Selected}' not found in the versions list.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static async Task StartGame(string gamePath, string dllDownload, string dllName)
        {
            AnsiConsole.MarkupLine("[blue]Starting Fortnite...[/]");
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher";
            string outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.txt");
            if (!File.Exists(gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe"))
                await DownloadFileAsync("https://www.dropbox.com/scl/fi/g9zq6w1xauufioes2zb4j/FortniteLauncher.exe?rlkey=7u5ib9b0swplxiz3phg6wcae6&st=ct93lwhm&dl=1", gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe");
            if (!File.Exists(appdata + "\\" + dllName))
                await DownloadFileAsync(dllDownload, appdata + "\\" + dllName);


            //Loading V1 Account System
            string emailPath = Path.Combine(appdata, "email.txt");
            string passwordPath = Path.Combine(appdata, "password.txt");

            string email = File.ReadAllText(emailPath);
            string password = File.ReadAllText(passwordPath);


            string launcherexePath = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncherPatch.exe";
            Process launcher = new Process();
            launcher.StartInfo.FileName = launcherexePath;
            launcher.StartInfo.WorkingDirectory = Path.GetDirectoryName(launcherexePath);
            launcher.StartInfo.Arguments = $" -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -nosplash -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={email} -AUTH_PASSWORD={password} -AUTH_TYPE=epic";
 


            Process shipping = new Process();
            shipping.StartInfo.FileName = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe";
            shipping.StartInfo.Arguments = $" -log -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -nosplash -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={email} -AUTH_PASSWORD={password} -AUTH_TYPE=epic";
            shipping.StartInfo.UseShellExecute = false;
            shipping.StartInfo.RedirectStandardOutput = true;
            shipping.StartInfo.RedirectStandardError = true;
            //  launcher.Start();
            //  await Task.Delay(5000); // Wait for the launcher to start the game
            // Process[] processes = Process.GetProcessesByName("FortniteClient-Win64-Shipping");
            //  if (processes.Length > 0)
            //  {
            //  XybLauncher.Injector.Inject(processes[0].Id, appdata + "\\" + dllName);
            // }
            shipping.Start();
            XybLauncher.Injector.Inject(shipping.Id, appdata + "\\" + dllName);
            Console.ReadLine();
        }

        private static async Task DownloadFileAsync(string url, string outputPath)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using (var fs = new FileStream(outputPath, FileMode.CreateNew))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
        }

        private class VersionData
        {
            public string Selected { get; set; }
            public Version[] Versions { get; set; }
        }

        private class Version
        {
            public int Index { get; set; }
            public string Season { get; set; }
            public string Path { get; set; }
        
        }



        public static async Task RunFortnite()
        {
            var fortniteManager = new StartHandler();

            string jsonFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";
            string dllDownload = "https://www.dropbox.com/scl/fi/m996mhjy77qn2t3bfxq6l/Cobalt.dll?rlkey=6araxm5ngyznp4fmvxqtgm9a7&st=x92p80dd&dl=0"; // Replace with actual URL
            string dllName = "Cobalt.dll";

            await fortniteManager.StartFortnite(jsonFilePath, dllDownload, dllName);
        }













    }
}
