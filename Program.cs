using Spectre.Console;

namespace XybLauncher;
// TODO Move files related stuff to another file

public static class Program
{


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


        // REDO
        VersionHandler.LoadSelectedSeason();
        VersionHandler.DisplaySelectedSeason();
        AccountHandler.ShowSelectedAccount();





        if (!Directory.Exists(appdata))
        {
            AnsiConsole.MarkupLine("[yellow]Appdata folder is missing, creating one [/]");
            Directory.CreateDirectory(appdata);
        }

        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]What do you want to do[/]?")
                .PageSize(6)
                .AddChoices(new[]
                {
                    "Start Client" ,"Start Server", "Add Fortnite Version", "Select Fortnite Version","Build Manager", "Download Build", "Account Manager", "Exit",
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
                    BuildDownloader filesManager = new BuildDownloader();
                    bool downloadSuccess = await filesManager.HandleActionAsync("DownloadBuild");

                    Console.WriteLine();
                    Console.WriteLine("Press Enter to return to main menu...");
                    Console.ReadLine();
                    AnsiConsole.Clear();  // Clear the console before showing menu again
                    await Main(args);     // Since Main is async, we need to await it
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    Console.WriteLine("Press Enter to return to main menu...");
                    Console.ReadLine();
                    AnsiConsole.Clear();
                    await Main(args);
                }
                break;  // Use break instead of return since we're in an async method






            case "Account Manager":
                AccountHandler.StartAccountManager();

                break;

            case "Exit":
                Environment.Exit(0);


                break;
        }
    }



    public static async Task MainMenu()
    {
        await Main(new string[0]);
    }




}
