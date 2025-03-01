using System.Diagnostics;
using System.Reflection;
using NLog;
using Spectre.Console;
using XybLauncher.Other;

namespace XybLauncher;
// TODO Move files related stuff to another file

public static class Program
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    //Variables for Season Selection 
    private static string selectedSeason = null;
    private static string selectedSeasonFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/selectedseason.json";
    private static string selectedPath = null;
    private static string versionsdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";



    public static async Task Main(string[] args)
    {
        //Variables
        string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher";
        string versionsdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";

        Console.Title = "XYB Launcher CLI";

        AnsiConsole.Clear();

        AnsiConsole.Write(
            new FigletText("XYB Launcher CLI")
                .Centered()
                .Color(Color.Blue)
        );

        AnsiConsole.Write(new Rule("[blue]Welcome to XYB Launcher CLI[/]").Centered());
        AnsiConsole.MarkupLine("Use the arrow keys [underline blue]UP[/] and [underline blue]DOWN[/] to navigate through the options.");

        Task.Run(() => DllsHandler.DownloadDefaultRedirects());
        Task.Run(() => DllsHandler.DownloadGameServers());
        ConfigHandler.ChkConfig();


        // REDO
        VersionHandler.LoadSelectedSeason();
        VersionHandler.DisplaySelectedSeason();
        AccountHandler.ShowSelectedAccount();
        DllsHandler.ShowSelectedLibraries();




        if (!Directory.Exists(appdata))
        {
            AnsiConsole.MarkupLine("[yellow]Appdata folder is missing, creating one [/]");
            Directory.CreateDirectory(appdata);
        }

        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]What do you want to do[/]")
                .PageSize(6)
                .AddChoices(new[]
                {
                    "Start Client" ,"Start Server", "Add Fortnite Version", "Select Fortnite Version","Build Manager", "Download Build", "Account Manager", "Settings" , "Exit",
                }));

        switch (option)
        {
            // Client Phase
            case "Start Client":
                AnsiConsole.Clear();
                Logger.Info("Starting the client");
                ClientManager.Test(args);
                break;

            // Server Phase
            case "Start Server":
                AnsiConsole.Clear();
                XybLauncher.ServerHandler.RunFortniteServer();

                AnsiConsole.MarkupLine("Starting the Server");
                Console.ReadLine();
                break;

            //Change Password Phase

            case "Change Fortnite Path":
                AnsiConsole.MarkupLine("Please enter the path to your fortnite folder");
                string setPath = AnsiConsole.Ask<string>("Path: ");
                string fortnitePath = Path.Combine(setPath, "FortniteGame", "Binaries", "Win64");

                if (Directory.Exists(fortnitePath))
                {
                    Console.WriteLine(fortnitePath);
                    File.WriteAllText(appdata + "\\path.txt", setPath);
                    Console.Clear();
                    AnsiConsole.MarkupLine("Path changed, you can now start the game");
                    Main(args);
                }
                else
                {
                    Console.WriteLine(fortnitePath);
                    AnsiConsole.MarkupLine("[red]Path is invalid![/]");
                    Main(args);
                }
                break;

            case "Add Fortnite Version":
                VersionHandler.AddFortniteVersion();
                AnsiConsole.Clear();
                Main(args);
                break;


            case "Select Fortnite Version":
                VersionHandler.SelectFortniteVersion();
                AnsiConsole.Clear();
                Program.MainMenu();
                break;


                // Fix to make it same as the account manager case
            case "Download Build":
                try
                {
                    BuildDownloader filesManager = new BuildDownloader();
                    bool downloadSuccess = await filesManager.HandleActionAsync("DownloadBuild");

                    Console.WriteLine();
                    Logger.Info("Press Enter to return to main menu...");
                    Console.ReadLine();
                    AnsiConsole.Clear();  // Clear the console before showing menu again
                    await Main(args);     // Since Main is async, we need to await it
                }
                catch (Exception ex)
                {
                    Logger.Error($"An unexpected error occurred: {ex.Message}");
                    Logger.Info("Press Enter to return to main menu...");
                    Console.ReadLine();
                    AnsiConsole.Clear();
                    await Main(args);
                }
                break;  // Use break instead of return since we're in an async method


            case "Settings":
                ConfigHandler.EditConfig();
                await MainMenu();  // Calling just mainmenu didn't work so we needed a await
                break;




            case "Account Manager":
                AccountHandler.StartAccountManager();
                await MainMenu();
                break;

            case "Exit":
                Environment.Exit(0);
                break;
        }
    }



    public static async Task MainMenu()
    {
        AnsiConsole.Clear();
        await Main(new string[0]);
    }




}
