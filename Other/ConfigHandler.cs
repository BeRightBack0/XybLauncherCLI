using System;
using System.IO;
using System.Linq;
using System.Net;
using IniParser;
using IniParser.Model;
using NLog;
using Spectre.Console;

namespace XybLauncher.Other
{
    public class ConfigHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string configfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XybLauncher/config.ini");
        private static string currentConfigVersion = "1.1.2";  // Default compiled version
        private static string latestConfigVersion = "1.1.2"; // Will be updated if an online version is found
        private static string dropboxConfigUrl = "https://www.dropbox.com/scl/fi/hksqkf89vtasvg5u1u3iv/config.ini?rlkey=wg6kfwk9ht1s9vbve5eu4dc82&st=webh7p9v&dl=1";
        private static FileIniDataParser parser = new FileIniDataParser();

        public static string GetSettingValue(string section, string key)
        {
            if (!File.Exists(configfile)) return null;

            try
            {
                IniData data = parser.ReadFile(configfile);
                if (data[section] != null && data[section][key] != null)
                {
                    string value = data[section][key];

                    // Remove inline comments (anything after ";") and quotes
                    return value.Split(';')[0].Trim().Trim('"');
                }
            }
            catch (Exception ex)
            {
                Logger.Debug($"Error reading setting [{section}] {key}: {ex.Message}");
            }

            return null;
        }

        public static void ChkConfig()
        {
            Logger.Debug("🔍 Checking config...");

            if (!File.Exists(configfile))
            {
                Logger.Debug("⚠ No local config found, downloading...");
                if (!DownloadAndSaveConfig())
                {
                    Logger.Debug("❌ No internet & no local config! Cannot proceed.");
                    throw new Exception("Config download required but no internet access.");
                }
            }

            var data = parser.ReadFile(configfile);
            currentConfigVersion = data["General"]["ConfigVersion"] ?? "Unknown";
            Logger.Debug($"📄 Current Config Version: {currentConfigVersion}");

            string latestConfigText = GetLatestConfigText();
            if (!string.IsNullOrEmpty(latestConfigText))
            {
                var latestData = parser.Parser.Parse(latestConfigText);
                latestConfigVersion = latestData["General"]["ConfigVersion"] ?? "Unknown";
                Logger.Debug($"🌍 Latest Config Version: {latestConfigVersion}");

                if (currentConfigVersion != latestConfigVersion)
                {
                    Logger.Debug($"🔄 Updating config from {currentConfigVersion} → {latestConfigVersion}...");
                    if (DownloadAndSaveConfig())
                    {
                        Logger.Debug("✅ Config updated successfully!");
                    }
                }
                else
                {
                    Logger.Debug("✔ Config is up to date.");
                }
            }
            else
            {
                Logger.Debug("⚠ Unable to check for the latest config. Using current version.");
            }
        }

        private static string GetLatestConfigText()
        {
            try
            {
                using WebClient client = new WebClient();
                return client.DownloadString(dropboxConfigUrl);
            }
            catch (Exception ex)
            {
                Logger.Debug($"⚠ Error checking latest config version: {ex.Message}");
                return null;
            }
        }

        private static bool DownloadAndSaveConfig()
        {
            try
            {
                using WebClient client = new WebClient();
                client.DownloadFile(dropboxConfigUrl, configfile);
                Logger.Debug("🚀 Config downloaded successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Debug($"⚠ Error downloading config: {ex.Message}");
                return false;
            }
        }

        public static void EditConfig()
        {
            // Sections to hide
            var hiddenSections = new HashSet<string>
            {
                "General",
                "VersionInfo"
            };

            // Settings that need a int
            var integerSettings = new Dictionary<string, (int Min, int Max)>
            {
                 {"ServerPort", (1024, 65535)}
            };

            while (true)
            {
                if (!File.Exists(configfile))
                {
                    AnsiConsole.MarkupLine("[red]⚠ Config file not found![/]");
                    return;
                }

                var data = parser.ReadFile(configfile);

                // Filter out hidden sections and add exit option
                var sections = data.Sections
                    .Select(s => s.SectionName)
                    .Where(s => !hiddenSections.Contains(s))
                    .ToList();
                sections.Add("Exit");
                AnsiConsole.Clear();

                AnsiConsole.Write(
                     new FigletText("XYB Launcher CLI")
                         .Centered()
                         .Color(Color.Blue)
                );
                // Select Section
                var section = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Select a setting:[/]")
                        .PageSize(15)
                        .AddChoices(sections));

                if (section == "Exit")
                {
                    break;
                }

                // Select Key
                var keys = data[section].Select(k => k.KeyName).ToList();
                keys.Add("Back");

                var key = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[blue]Select a key from {section}:[/]")
                        .PageSize(10)
                        .AddChoices(keys));

                if (key == "Back")
                {
                    continue;
                }

                string currentValue = data[section][key].ToString()?.Trim('"') ?? "N/A";
                AnsiConsole.MarkupLine($"[green]Current Value: {currentValue}[/]");

                // Check if this setting requires integer validation
                var isIntSetting = integerSettings.TryGetValue(key, out var range);

                if (isIntSetting)
                {
                    while (true)
                    {
                        var input = AnsiConsole.Ask<string>($"[blue]Enter a number between {range.Min} and {range.Max} (or 'cancel' to go back):[/]");

                        if (input.ToLower() == "cancel")
                        {
                            break;
                        }

                        if (int.TryParse(input, out int numValue) && numValue >= range.Min && numValue <= range.Max)
                        {
                            data[section][key] = input;
                            parser.WriteFile(configfile, data);
                            AnsiConsole.MarkupLine($"[green]✅ Updated {section}.{key} to: {input}[/]");
                            break;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"[red]Please enter a valid number between {range.Min} and {range.Max}[/]");
                        }
                    }
                }
                else
                {
                    // Regular string setting - check for options in comments
                    var options = GetOptionsFromComments(data, section, key);
                    string newValue;

                    if (options.Any())
                    {
                        options.Add("Cancel");

                        newValue = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[blue]Select a value:[/]")
                                .PageSize(10)
                                .AddChoices(options));

                        if (newValue == "Cancel")
                        {
                            continue;
                        }

                        if (data[section][key].ToString().StartsWith("\""))
                        {
                            newValue = $"\"{newValue}\"";
                        }
                    }
                    else
                    {
                        newValue = AnsiConsole.Ask<string>("[blue]Enter new value (or 'cancel' to go back):[/]");

                        if (newValue.ToLower() == "cancel")
                        {
                            continue;
                        }
                    }

                    data[section][key] = newValue;
                    parser.WriteFile(configfile, data);
                    AnsiConsole.MarkupLine($"[green]✅ Updated {section}.{key} to: {newValue}[/]");
                }
            }
        }

        private static List<string> GetOptionsFromComments(IniData data, string section, string key)
        {
            var options = new List<string>();

            var keyData = data.Sections[section].GetKeyData(key);
            if (keyData?.Comments != null)
            {
                foreach (var comment in keyData.Comments)
                {
                    var cleanComment = comment.TrimStart('#', ';', ' ');

                    if (cleanComment.Contains("Available options:", StringComparison.OrdinalIgnoreCase))
                    {
                        var optionsStr = cleanComment.Substring(
                            cleanComment.IndexOf(":", StringComparison.OrdinalIgnoreCase) + 1);
                        options.AddRange(optionsStr.Split(',').Select(o => o.Trim()));
                        break;
                    }
                    else if (cleanComment.StartsWith("options=", StringComparison.OrdinalIgnoreCase))
                    {
                        var optionsStr = cleanComment.Substring(8).Trim();
                        options.AddRange(optionsStr.Split(',').Select(o => o.Trim()));
                        break;
                    }
                }
            }

            return options.Select(o => o.Trim().Trim('"')).Distinct().ToList();
        }



    }
}
