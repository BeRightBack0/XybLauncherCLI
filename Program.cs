using System.Diagnostics;
using Newtonsoft.Json;
using Spectre.Console;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Terminal.Gui.Graphs.PathAnnotation;
using static XybLauncher.FilesManager;
using static XybLauncher.StartHandler;
using static XybLauncher.AccountHandler;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace XybLauncher;

public static class Program
{


    //Variables

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

        VersionHandler.LoadSelectedSeason();
        VersionHandler.DisplaySelectedSeason();
        AccountHandler.ShowSelectedAccount();





        if (!Directory.Exists(appdata))
        {
            AnsiConsole.MarkupLine("[yellow]Appdata folder is missing, creating one for you...[/]");
            Directory.CreateDirectory(appdata);
        }

        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]What do you want to do[/]?")
                .PageSize(6)
                .AddChoices(new[]
                {
                    "Start Client" ,"Start Server", "Add Fortnite Version", "Select Fortnite Version",  "Download Build", "Account Manager", "Exit",
                }));

        switch (option)
        {
            // Client Phase
            case "Start Client":
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("Starting the client");
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

            //case "Exit":
               // Environment.Exit(1);
               // break;


            case "Change Account Info":
                AnsiConsole.MarkupLine("[red]Please Enter Your Email and Password![/]");

                Console.Write("Email: ");
                string email = Console.ReadLine();
                File.WriteAllText(appdata + "\\email.txt", email);

                Console.Write("Password: ");
                string password = Console.ReadLine();
                File.WriteAllText(appdata + "\\password.txt", password);

                AnsiConsole.MarkupLine("[green]Email and Password Saved![/]");
                break;

            case "Add Fortnite Version":
                VersionHandler.AddFortniteVersion();
                AnsiConsole.Clear();
                Main(args);
                break;


            case "Select Fortnite Version":
                VersionHandler.SelectFortniteVersion();
                AnsiConsole.Clear();
                Main(args);
                break;

            case "Download Build":
                try
                {
                    Console.WriteLine("Starting DownloadBuild process...");

                    // Instantiate FilesManager
                    FilesManager filesManager = new FilesManager();


                    // Call the main logic method in FilesManager
                    await filesManager.HandleActionAsync("DownloadBuild");

                    // Log after the method call
                    Console.WriteLine("HandleActionAsync completed.");

                    // Indicate that the process has completed
                    Console.WriteLine("Download completed successfully. Press Enter to exit.");
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
                finally
                {
                    // Keep the console window open
                    Console.ReadLine();
                }
                break;



            case "Account Manager":
                AccountHandler.StartAccountManager();

            break;

            case "Exit":
                Environment.Exit(0);


                break;
        }
    }



    public static void MainMenu()
    {
        Program.Main(new string[0]);
    }



}



















