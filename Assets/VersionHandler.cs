using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Text;
using System.Threading.Tasks;
//TODO Name it better and tranfer all function for versions selection and handling here ok and downloading builds herre ok ok ok
namespace ChannelLauncher
{
    internal class VersionHandler
    {
        //Strings
        private static string selectedSeason = null;
        private static string selectedSeasonFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/selectedseason.json";
        private static string selectedPath = null;
        private static string versionsdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";

        public class FortniteVersion
        {
            public string Season { get; set; }
            public string Path { get; set; }
        }

        public class FortniteVersions
        {
            public string Selected { get; set; }
            public List<FortniteVersion> Versions { get; set; } = new List<FortniteVersion>();
        }





        static void DisplaySelectedSeason()
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
                var data = JsonConvert.DeserializeObject<MomentumLauncher.VersionHandler.versionhandler.FortniteVersions>(jsonData);

                // Check if there is a selected season
                if (!string.IsNullOrEmpty(data.Selected))
                {
                    Console.WriteLine($"Selected Version: {data.Selected}");
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

    }
}
