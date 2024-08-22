using System.Diagnostics;
using Newtonsoft.Json;
using Spectre.Console;
using System.Net;
using System.Threading.Tasks;
using static Terminal.Gui.Graphs.PathAnnotation;

namespace ChannelLauncher;

public static class Program
{

    private static string selectedSeason = null;
    public static async Task Main(string[] args)
    {
        Console.Title = "XYB Launcher CLI";



        AnsiConsole.MarkupLine("Welcome, " + Environment.UserName + ", to the [underline blue]XYB Launcher CLI.[/]");
        AnsiConsole.MarkupLine("You can select an option using the arrow keys [underline blue]UP[/] and [underline blue]DOWN.[/]");

        if (!string.IsNullOrEmpty(selectedSeason))
        {
            Console.WriteLine($"[Selected Season: {selectedSeason}]");
        }
        else
        {
            Console.WriteLine("[No Season Selected]");
        }

        string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher";

        string versionsdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";


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
                AnsiConsole.MarkupLine("[red]Please Enter Your Fortnite Path And Season Number[/]");
                Console.Write("Fornite Path: ");
                string fortnitepath = Console.ReadLine();

                Console.Write("Which Season Is It: ");
                string fortniteversion = Console.ReadLine();

                // Combine the path and version information
                string fortniteversioninfo = $"{fortnitepath} {fortniteversion}";

                // Append the combined information to the file, adding a newline character
                File.AppendAllText(appdata + "\\versions.json", fortniteversioninfo + Environment.NewLine);
                AnsiConsole.Clear();
                Main(args);
                break;


            case "Select Fortnite Version":
                string selectedVersion = SelectFortniteVersion(versionsdata);
                if (selectedVersion != null)
                {
                    Console.WriteLine($"You selected: {selectedVersion}");
                }

                break;
        }
    }

    static string SelectFortniteVersion(string versionsdata)
    {
        string[] lines = File.ReadAllLines(versionsdata);
        Console.WriteLine("\n--- Available Fortnite Versions ---");
        for (int i = 0; i < lines.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {lines[i]}");
        }

        Console.Write("Select a version by number: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= lines.Length)
        {
            return lines[index - 1];
        }
        else
        {
            Console.WriteLine("Invalid selection.");
            return null;
        }
    }
}


