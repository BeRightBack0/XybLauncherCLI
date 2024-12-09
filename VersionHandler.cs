using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static XybLauncher.VersionHandler;
using Spectre.Console;

//TODO Name it better and tranfer all function for versions selection and handling here ok and downloading builds herre ok ok ok
namespace XybLauncher;

internal class VersionHandler
{
    //Strings
    private static string selectedSeason = null;
    private static string selectedSeasonFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/selectedseason.json";
    private static string selectedPath = null;
    private static string versionsdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";

    public class FortniteVersion
    {
        public int Index { get; set; }
        public string Season { get; set; }
        public string Path { get; set; }
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
            // Read the JSON file
            var jsonData = File.ReadAllText(versionsdata);

            // Deserialize the JSON data into the FortniteVersions object
            var data = JsonConvert.DeserializeObject<FortniteVersions>(jsonData);

            // Check if there is a selected season
            if (!string.IsNullOrEmpty(data.Selected))
            {
                AnsiConsole.Markup($"Selected Version: [green]{data.Selected}[/]");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No season selected.");
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

        // Determine the new index
        int newIndex = data.Versions.Count + 1;

        // Create a new FortniteVersion object
        var newVersion = new FortniteVersion
        {
            Index = newIndex,
            Season = fortniteSeason,
            Path = fortnitePath
        };

        // Add the new version to the list of versions
        data.Versions.Add(newVersion);

        // Serialize the updated data back to JSON
        var updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Write the updated JSON data back to the file
        File.WriteAllText(versionsdata, updatedJson);

        Console.WriteLine($"Fortnite version added successfully with index {newIndex}!");
    }






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

    static void SaveSelectedSeason()
    {
        // Save the selected season and path to a file
        File.WriteAllText(selectedSeasonFilePath, $"{selectedSeason} {selectedPath}");
    }



}
