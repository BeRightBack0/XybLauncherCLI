using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace XybLauncher
{
    public class ClientManager
    {

        private static string versionsdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";
        private static Process _fnProcess;
        private static Patcher _fnPatcher;


        public static string GetFortniteExecutablePath(string versionsData)
        {
            string FORTNITE_EXECUTABLE_RELATIVE = "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe";
            string selectedGamePath = PathParser.GetSelectedPath(versionsData); // Replace with your actual implementation
            string fullPath = Path.Combine(selectedGamePath, FORTNITE_EXECUTABLE_RELATIVE);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"The Fortnite executable was not found at: {fullPath}");
            }

            return fullPath;
        }





        //Main function for starting fortnite
        public static void Main(string[] args)
        {
            string versionsData = "versions.json"; // Path to the JSON file
            string fullPath;

            try
            {
                // Get the full path of the Fortnite executable
                fullPath = GetFortniteExecutablePath(versionsData);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }

            string joinedArgs = string.Join(" ", args);

            // Check if -FORCECONSOLE exists in args
            if (joinedArgs.ToUpper().Contains("-FORCECONSOLE"))
            {
                joinedArgs = Regex.Replace(joinedArgs, "-FORCECONSOLE", string.Empty, RegexOptions.IgnoreCase);
                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.GetFileName(Assembly.GetEntryAssembly().Location),
                        Arguments = joinedArgs,
                        UseShellExecute = false
                    }
                }.Start();

                Environment.Exit(0);
            }

            // Check if -NOSSLPINNING exists in args
            if (joinedArgs.ToUpper().Contains("-NOSSLPINNING"))
            {
                joinedArgs = Regex.Replace(joinedArgs, "-NOSSLPINNING", string.Empty, RegexOptions.IgnoreCase);
                Patcher.noSslPinning = true;
            }

            // Setup a process exit event handler
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            // Initialize Fortnite process with start info
            _fnProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fullPath,
                    Arguments = $"{joinedArgs} -log  -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN=birajtsrak@outlook.com -AUTH_PASSWORD=12345678 -AUTH_TYPE=epic",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            _fnProcess.Start(); // Start Fortnite client process

            _fnProcess.WaitForExit(); // Wait for the process to exit
        }


        //Something To Close The Game Idk it works so it works
        private static void OnProcessExit(object sender, EventArgs e)
        {
            if (_fnProcess != null && !_fnProcess.HasExited)
            {
                _fnProcess.Kill();
            }
        }











































    }



}
