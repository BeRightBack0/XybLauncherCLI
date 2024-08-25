using System.Diagnostics;
using Newtonsoft.Json;
using Spectre.Console;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Terminal.Gui.Graphs.PathAnnotation;

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
        LoadSelectedSeason();
        AnsiConsole.MarkupLine("Welcome, " + Environment.UserName + ", to the [underline blue]XYB Launcher CLI.[/]");
        AnsiConsole.MarkupLine("You can select an option using the arrow keys [underline blue]UP[/] and [underline blue]DOWN.[/]");

        // Display the selected season and path if available
        DisplaySelectedSeason();



        if (!Directory.Exists(appdata))
        {
            AnsiConsole.MarkupLine("[yellow]Appdata folder is missing, creating one for you...[/]");
            Directory.CreateDirectory(appdata);
        }

        var option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]What do you want to do[/]?")
                .PageSize(5)
                .AddChoices(new[]
                {
                    "Start Client", "Add Fortnite Version", "Select Fortnite Version",  "Change Fortnite Path","Change Account Info", "Exit"
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

            case "Exit":
                Environment.Exit(1);
                break;


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
                AddFortniteVersion();
                AnsiConsole.Clear();
                Main(args);
                break;


            case "Select Fortnite Version":
                SelectFortniteVersion();
                AnsiConsole.Clear();
                Main(args);
                break;
        }
    }

    static void SaveSelectedSeason()
    {
        // Save the selected season and path to a file
        File.WriteAllText(selectedSeasonFilePath, $"{selectedSeason} {selectedPath}");
    }


    static void LoadSelectedSeason()
    {
        // Load the selected season and path from a file if it exists
        if (File.Exists(selectedSeasonFilePath))
        {
            string[] parts = File.ReadAllText(selectedSeasonFilePath).Split(new[] { ' ' }, 2);
            if (parts.Length == 2)
            {
                selectedSeason = parts[0];
                selectedPath = parts[1];
            }
        }
    }



    static void AddFortniteVersion()
    {
        Console.WriteLine("Adding a new Fortnite version...");

        // Get the path and season from the user
        Console.Write("Enter Fortnite path: ");
        string fortnitePath = Console.ReadLine();

        Console.Write("Enter Fortnite season: ");
        string fortniteSeason = Console.ReadLine();

        // Create a new FortniteVersion object
        var newVersion = new FortniteVersion
        {
            Season = fortniteSeason,
            Path = fortnitePath
        };

        // Load existing data or create new structure
        FortniteVersions data;
        if (File.Exists(versionsdata))
        {
            // Read existing JSON data from the file
             var jsonData = File.ReadAllText(versionsdata);
            // Deserialize JSON data into FortniteVersions object
            data = JsonConvert.DeserializeObject<FortniteVersions>(jsonData) ?? new FortniteVersions();
        }
        else
        {
            // If the file doesn't exist, create a new FortniteVersions object
            data = new FortniteVersions();
        }

        // Add the new version to the list of versions
        data.Versions.Add(newVersion);

        // Serialize the updated data back to JSON
        var updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Write the updated JSON data back to the file
        File.WriteAllText(versionsdata, updatedJson);

        Console.WriteLine("Fortnite version added successfully!");
    }



    static void SelectFortniteVersion()
    {
        // Check if the file exists
        if (!File.Exists(versionsdata))
        {
            Console.WriteLine("No versions found. Please add a version first.");
            return;
        }

        FortniteVersions data;

        try
        {
            // Read the JSON file
            var jsonData = File.ReadAllText(versionsdata);

            // Deserialize the JSON data into the FortniteVersions object
            data = JsonConvert.DeserializeObject<FortniteVersions>(jsonData);

            if (data == null || data.Versions.Count == 0)
            {
                Console.WriteLine("No versions found in the file.");
                return;
            }

            // Display all versions
            Console.WriteLine("\n--- Available Fortnite Versions ---");
            for (int i = 0; i < data.Versions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Version: {data.Versions[i].Season} Path: {data.Versions[i].Path}");
            }

            // Select a version
            Console.Write("Select a version by number: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= data.Versions.Count)
            {
                // Mark the selected version
                var selectedVersion = data.Versions[index - 1];
                data.Selected = selectedVersion.Season;

                // Serialize the updated data back to JSON

                var updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(versionsdata, updatedJson);

                Console.WriteLine($"Selected Version: Season {selectedVersion.Season}, Path: {selectedVersion.Path}");
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading or writing JSON data: {ex.Message}");
        }
    }

   
}



















