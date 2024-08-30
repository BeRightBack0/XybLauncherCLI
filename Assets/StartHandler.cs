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
using static ChannelLauncher.Program;
using System.Net.Http;
using System.Text.Json.Serialization;




namespace ChannelLauncher
{
    public class StartHandler
    {
        private string selectedPath;


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




        public static void DownloadFile(string Url, string Path) => new WebClient().DownloadFile(Url, Path);





        public void ReadSelectedVersionPath(string jsonFilePath)
        {

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"Error: File does not exist at path: {jsonFilePath}");
                return;
            }

            try
            {
                // Read the JSON file
                string jsonContent = File.ReadAllText(jsonFilePath);

                // Deserialize the JSON content
                var versionData = JsonConvert.DeserializeObject<VersionData>(jsonContent);

                // Find the version that matches the Selected property
                var selectedVersion = versionData.Versions.FirstOrDefault(v => v.Season == versionData.Selected);

                if (selectedVersion != null)
                {
                    // Store the path in memory
                    selectedPath = selectedVersion.Path;
                    Console.WriteLine($"Selected path: {selectedPath}");
                }
                else
                {
                    Console.WriteLine($"Error: Selected version '{versionData.Selected}' not found in the versions list.");
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Error parsing JSON: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }


















        public static async void StartGame(string gamePath, string dllDownload, string dllName)
        {
            AnsiConsole.MarkupLine("[blue]Starting Fortnite...[/]");

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher";
            string outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.txt");

            if (!File.Exists(gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe"))
                DownloadFile("https://www.dropbox.com/scl/fi/g9zq6w1xauufioes2zb4j/FortniteLauncher.exe?rlkey=7u5ib9b0swplxiz3phg6wcae6&st=ct93lwhm&dl=1", gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe");
            if (!File.Exists(appdata + "\\" + dllName))
                DownloadFile(dllDownload, appdata + "\\" + dllName);

            string emailPath = Path.Combine(appdata, "email.txt");
            string passwordPath = Path.Combine(appdata, "password.txt");

            string email = File.ReadAllText(emailPath);
            string password = File.ReadAllText(passwordPath);

            string launcherexePath = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncherPatch.exe";
            Process launcher = new Process();
            launcher.StartInfo.FileName = launcherexePath;
            launcher.StartInfo.WorkingDirectory = Path.GetDirectoryName(launcherexePath);
            launcher.StartInfo.Arguments = $" -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -nosplash -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={email} -AUTH_PASSWORD={password} -AUTH_TYPE=epic";


            Process shippingbe = new Process();
            shippingbe.StartInfo.FileName = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe";

            Process shipping = new Process();
            shipping.StartInfo.FileName = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe";
            shipping.StartInfo.Arguments = $" -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -nosplash -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={email} -AUTH_PASSWORD={password} -AUTH_TYPE=epic";
            shipping.StartInfo.UseShellExecute = false;
            shipping.StartInfo.RedirectStandardOutput = true;
            shipping.StartInfo.RedirectStandardError = true;

            //  launcher.Start();
            //  await Task.Delay(5000); // Wait for the launcher to start the game

            // Process[] processes = Process.GetProcessesByName("FortniteClient-Win64-Shipping");
            //  if (processes.Length > 0)
            //  {
            //  MomentumLauncher.Injector.Inject(processes[0].Id, appdata + "\\" + dllName);
            // }



            shipping.Start();
            MomentumLauncher.Injector.Inject(shipping.Id, appdata + "\\" + dllName);



            Environment.Exit(0);
        }


















    }
}
