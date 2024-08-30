using System.Diagnostics;
using Newtonsoft.Json;
using Spectre.Console;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Terminal.Gui.Graphs.PathAnnotation;
using static ChannelLauncher.FilesManager;
using static ChannelLauncher.StartHandler;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace ChannelLauncher;

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
        AnsiConsole.MarkupLine($"[bold yellow]Welcome, {Environment.UserName}, to the [underline blue]XYB Launcher CLI[/].[/]");
        AnsiConsole.MarkupLine("Use the arrow keys [underline blue]UP[/] and [underline blue]DOWN[/] to navigate through the options.");

        VersionHandler.LoadSelectedSeason();
        VersionHandler.DisplaySelectedSeason();





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
                    "Start Client", "Add Fortnite Version", "Select Fortnite Version",  "Download Build", "Change Account Info", "Exit",
                }));

        switch (option)
        {
            // Client Phase
            case "Start Client":

                if (!File.Exists(appdata + "\\path.txt"))
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine("[red]Fortnite path is missing, please set it first![/]");
                    Main(args);
                }

                if (!File.Exists(appdata + "\\email.txt") || !File.Exists(appdata + "\\password.txt"))
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine("[red]Please Enter Your Email and Password! (Account Made From Discord Bot.)[/]");

                    Console.Write("Email: ");
                    string clientemail = Console.ReadLine();
                    File.WriteAllText(appdata + "\\email.txt", clientemail);

                    Console.Write("Password: ");
                    string clientpassword = Console.ReadLine();
                    File.WriteAllText(appdata + "\\password.txt", clientpassword);

                    AnsiConsole.MarkupLine("[green]Email and Password Saved![/]");
                }

                if (!File.Exists("redirect.json"))
                {
                    string fileContent = "{ \"name\": \"Cobalt.dll\", \"download\": \"https://www.dropbox.com/scl/fi/m996mhjy77qn2t3bfxq6l/Cobalt.dll?rlkey=6araxm5ngyznp4fmvxqtgm9a7&st=fo0dke5u&dl=1\" }";
                    File.WriteAllText("redirect.json", fileContent);
                }

                string fileData = File.ReadAllText("redirect.json");

                var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileData);

                if (!jsonData.TryGetValue("name", out var dllNameObject) || !(dllNameObject is string dllName))
                {
                    throw new Exception("Invalid JSON structure: 'name' key is missing or not a string");
                }

                if (!jsonData.TryGetValue("download", out var dllDownloadObject) || !(dllDownloadObject is string dllDownload))
                {
                    throw new Exception("Invalid JSON structure: 'download' key is missing or not a string");
                }

                if (!dllName.EndsWith(".dll"))
                {
                    dllName += ".dll";
                }

                MomentumLauncher.Utilities.StartGame(File.ReadAllText(appdata + "\\path.txt"), dllDownload, dllName);
                AnsiConsole.MarkupLine("Starting the client");
                break;

            // Server Phase
            case "Start Server":

                if (!File.Exists(appdata + "\\path.txt"))
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine("[red]Fortnite path is missing, please set it first![/]");
                    Main(args);
                }

                if (!File.Exists(appdata + "\\serveremail.txt") || !File.Exists(appdata + "\\serverpassword.txt"))
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine("[red]Please Enter Your Server Email and Server Password! (Account Made From Discord Bot.)[/]");

                    Console.Write("Server Email: ");
                    string serveremail = Console.ReadLine();
                    File.WriteAllText(appdata + "\\serveremail.txt", serveremail);

                    Console.Write("Server Password: ");
                    string serverpassword = Console.ReadLine();
                    File.WriteAllText(appdata + "\\serverpassword.txt", serverpassword);

                    AnsiConsole.MarkupLine("[green] Server Email and Password Saved![/]");
                }

                if (!File.Exists("redirect.json"))
                {
                    string fileContent = "{ \"name\": \"Cobalt.dll\", \"download\": \"https://www.dropbox.com/scl/fi/m996mhjy77qn2t3bfxq6l/Cobalt.dll?rlkey=6araxm5ngyznp4fmvxqtgm9a7&st=fo0dke5u&dl=1\" }";
                    File.WriteAllText("redirect.json", fileContent);
                }

                string serverFileData = File.ReadAllText("redirect.json");

                var serverJsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(serverFileData);

                if (!serverJsonData.TryGetValue("name", out var serverDllNameObject) || !(serverDllNameObject is string serverDllName))
                {
                    throw new Exception("Invalid JSON structure: 'name' key is missing or not a string");
                }

                if (!serverJsonData.TryGetValue("download", out var serverDllDownloadObject) || !(serverDllDownloadObject is string serverDllDownload))
                {
                    throw new Exception("Invalid JSON structure: 'download' key is missing or not a string");
                }

                if (!serverDllName.EndsWith(".dll"))
                {
                    serverDllName += ".dll";
                }

                MomentumLauncher.Utilities.StartServer(File.ReadAllText(appdata + "\\path.txt"), serverDllDownload, serverDllName);
                AnsiConsole.MarkupLine("Starting the Server");
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





            case "Exit":
                AnsiConsole.Clear();
                ChannelLauncher.StartHandler.RunFortnite();


                break;
        }
    }




   
}



















