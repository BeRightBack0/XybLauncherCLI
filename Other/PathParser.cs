using Newtonsoft.Json;
using System;
using System.IO;

namespace XybLauncher
{
    public static class PathParser
    {
        public class Version
        {
            public int Index { get; set; }
            public string Season { get; set; }
            public string Path { get; set; }
        }

        public class VersionInfo
        {
            public string Selected { get; set; }
            public Version[] Versions { get; set; }
        }

        public static string GetSelectedPath(string jsonFilePath)
        {
            // Check if the JSON file exists
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"The file \"{jsonFilePath}\" does not exist.");
            }

            // Read and deserialize the JSON
            string jsonContent = File.ReadAllText(jsonFilePath);
            VersionInfo versionInfo = JsonConvert.DeserializeObject<VersionInfo>(jsonContent);

            // Find the selected version's path
            foreach (var version in versionInfo.Versions)
            {
                if (version.Season == versionInfo.Selected)
                {
                    return version.Path;
                }
            }

            throw new Exception("Selected version not found in the JSON file.");
        }
    }
}
