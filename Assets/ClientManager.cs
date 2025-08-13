using Newtonsoft.Json;
using NLog;
using Spectre.Console;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using XybLauncher.Other;

namespace XybLauncher
{
    public class ClientManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string versionsdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XybLauncher", "versions.json");

        private static Process _fnProcess;
        private static Patcher _fnPatcher;

        public static void Test(string[] args)
        {
            string buildarchitecture = XybLauncher.CustomArguments.GetBuildArchitecture();
            string selectedPath = PathParser.GetSelectedPath(versionsdata);

            string fortniteExecutable = $"{selectedPath}\\FortniteGame\\Binaries\\{buildarchitecture}\\FortniteClient-Win64-Shipping.exe";

            string joinedArgs = string.Join(" ", args);
            string appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XybLauncher");

            // Check if -FORCECONSOLE exists in args (regardless of case) to force console (due to Epic Games Launcher by default hiding it)
            if (joinedArgs.ToUpper().Contains("-FORCECONSOLE"))
            {
                joinedArgs = Regex.Replace(joinedArgs, "-FORCECONSOLE", string.Empty, RegexOptions.IgnoreCase);
                new Process
                {
                    StartInfo =
                    {
                        FileName = Path.GetFileName(Assembly.GetEntryAssembly().Location),
                        Arguments = joinedArgs,
                        UseShellExecute = false,
                    },
                }.Start();

                Environment.Exit(0);
            }

            // Check if the Fortnite client exists in the current work path.
            if (!File.Exists(fortniteExecutable))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Logger.Error($"\"{fortniteExecutable}\" is missing!");
                Console.ReadKey();
                Program.MainMenu();
            }

            // Check if -NOSSLPINNING exists in args (regardless of case) to disable SSL pinning
            if (joinedArgs.ToUpper().Contains("-NOSSLPINNING"))
            {
                joinedArgs = Regex.Replace(joinedArgs, "-NOSSLPINNING", string.Empty, RegexOptions.IgnoreCase);
                Patcher.noSslPinning = true;
            }

            // Setup a process exit event handler
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            var accountParser = new AccountParser();
            string email = accountParser.GetAccountInfo("email");
            string password = accountParser.GetAccountInfo("password");

            CustomArguments customArgs = new CustomArguments();
            string defaultargs = customArgs.GetDefaultArgs(email, password);

            // Initialize Fortnite process with start info
            _fnProcess = new Process
            {
                StartInfo =
                {
                    FileName = fortniteExecutable,
                    Arguments = $"{joinedArgs} {defaultargs}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                },
            };

            _fnProcess.Start(); // Start Fortnite client process

            // im too lazy to implement so it will change the dll depending on the ot selection
            // Injector.Inject(_fnProcess.Id, Path.Combine(appdata, "Cobalt.dll"));
            //System.Threading.Thread.Sleep(15000); // 15000 ms = 15 seconds
            Injector.Inject(_fnProcess.Id, Path.Combine(appdata, "Cobalt.dll"));




            // Set up our async readers
            AsyncStreamReader asyncOutputReader = new AsyncStreamReader(_fnProcess.StandardOutput);
            AsyncStreamReader asyncErrorReader = new AsyncStreamReader(_fnProcess.StandardError);

            asyncOutputReader.DataReceived += delegate (object sender, string data)
            {
                if (data == null)
                {
                    AnsiConsole.Clear();
                    XybLauncher.Program.MainMenu();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.White;

                string formattedData = data.ToUpper().Replace(" ", "_"); // Convert data to all uppercase characters and replace spaces with "_"

                // Check if formatted data contains "ASYNC_LOADING_INITIALIZED", if so, initialize the patcher (because we have to wait for Fortnite to be fully loaded into memory)
                if (formattedData.Contains("ASYNC_LOADING_INITIALIZED") && _fnPatcher == null)
                    _fnPatcher = new Patcher(_fnProcess);

                // Check if formatted data contains "STARTING_UPDATE_CHECK", if so, run the patcher (because Fortnite internal AC sucks!)
                if (formattedData.Contains("STARTING_UPDATE_CHECK") && _fnPatcher != null)
                    _fnPatcher.Run();

                Console.WriteLine(data);
            };

            asyncErrorReader.DataReceived += delegate (object sender, string data)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(data);
            };

            // Start our async readers
            asyncOutputReader.Start();
            asyncErrorReader.Start();

            _fnProcess.WaitForExit(); // We'll wait for the Fortnite process to exit, otherwise the launcher will close
        }

        private static async void OnProcessExit(object sender, EventArgs e)
        {
            if (!_fnProcess.HasExited)
                _fnProcess.Kill();
            Console.ReadKey();
        }
    }
}