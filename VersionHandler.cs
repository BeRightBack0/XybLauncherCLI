using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static XybLauncher.VersionHandler;
using Spectre.Console;

// Just make the code readable
namespace XybLauncher;

internal class VersionHandler
{

    private static string selectedSeason = null;
    private static string selectedSeasonFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/selectedseason.json";
    private static string selectedPath = null;
    private static string versionsdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";

    public class FortniteVersion
    {
        public int Index { get; set; }
        public string Season { get; set; }
        public string Path { get; set; }
        public bool IsOnlineTest { get; set; }
       

    }

    public class FortniteVersions
    {
        public string Selected { get; set; }

        public List<FortniteVersion> Versions { get; set; } = new List<FortniteVersion>();
    }






    public static void DisplaySelectedSeason()
    {
        if (!File.Exists(versionsdata))
        {
            Console.WriteLine("No versions file found. Please add a version first.");
            return;
        }

        try
        {
            var jsonData = File.ReadAllText(versionsdata);

            // Deserialize the JSON data into the FortniteVersions object
            var data = JsonConvert.DeserializeObject<FortniteVersions>(jsonData);

            // Check if there is a selected build
            if (!string.IsNullOrEmpty(data.Selected))
            {
                AnsiConsole.Markup($"Selected Version: [blue]{data.Selected}[/]");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No build selected.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading JSON data: {ex.Message}");
        }
    }
    public static void SelectFortniteVersion()
    {
        // Check if the file exists
        if (!File.Exists(versionsdata))
        {
            Console.WriteLine("No versions found. Please add a build first.");
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
            foreach (var version in data.Versions)
            {
                Console.WriteLine($"{version.Index}. Version: {version.Season} Path: {version.Path}");
            }

            // Select a version
            Console.Write("Select a version by number: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= data.Versions.Count)
            {
                // Find the selected version by index
                var selectedVersion = data.Versions.FirstOrDefault(v => v.Index == index);
                if (selectedVersion != null)
                {
                    // Mark the selected version
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




    public static void AddFortniteVersion()
    {
        Console.WriteLine("Adding a new Fortnite version...");

        // Get the path and season from the user
        Console.Write("Enter Fortnite path: ");
        string fortnitePath = Console.ReadLine();

        Console.Write("Enter Fortnite season: ");
        string fortniteSeason = Console.ReadLine();


        // This should be changed to automatically detect if the build is 32bit if yes then use online test args if no then use default ones
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Is the Build Online Test?")
                .PageSize(3)
                .AddChoices(new[] { "Yes", "No" })
                .HighlightStyle(new Style(Color.Blue))
        );

        bool isOnlineTest = selection == "Yes";

        AddFortniteVersionEntry(fortnitePath, fortniteSeason, isOnlineTest ? 1 : 0);
    }

    public static void AddFortniteVersionEntry(string path, string season, int onlineTest)
    {
        // Load existing data or create new structure
        FortniteVersions data = File.Exists(versionsdata)
            ? JsonConvert.DeserializeObject<FortniteVersions>(File.ReadAllText(versionsdata)) ?? new FortniteVersions()
            : new FortniteVersions();

        // Determine the new index
        int newIndex = data.Versions.Count + 1;

        // Create a new FortniteVersion object
        var newVersion = new FortniteVersion
        {
            Index = newIndex,
            Season = season,
            Path = path,
            IsOnlineTest = onlineTest == 1
        };

        // Add the new version to the list of versions
        data.Versions.Add(newVersion);

        // Serialize the updated data back to JSON
        File.WriteAllText(versionsdata, JsonConvert.SerializeObject(data, Formatting.Indented));

        Console.WriteLine($"Fortnite version added successfully with index {newIndex}!");
    }







    // Idk if its used but i think its not so maybe clean it up later
    public static void LoadSelectedSeason()
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



}
