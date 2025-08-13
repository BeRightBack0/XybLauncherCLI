using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spectre.Console;
using System.Text.RegularExpressions;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using static XybLauncher.Assets.Logging;
using NLog;

namespace XybLauncher
{
    public class BuildDownloader
    {
        private readonly HttpClient _httpClient;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private bool _downloadCompleted;

        public BuildDownloader()
        {
            _httpClient = new HttpClient();
            _downloadCompleted = false;
        }

        public async Task<bool> HandleActionAsync(string action)
        {
            if (action == "DownloadBuild")
            {
                try
                {
                    string apiUrl2 = "https://api.xybnetwork.xyz//builds";
                    string apiUrl = "https://pastebin.com/raw/GbaitDKE";
                    using HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var buildData = JsonConvert.DeserializeObject<Dictionary<string, List<BuildInfo>>>(jsonResponse);

                    if (buildData == null || buildData.Count == 0)
                    {
                        Logger.Error("API is down");
                        return false;
                    }

                    var versions = buildData
                        .Where(kv => kv.Value.Any())
                        .ToDictionary(kv => kv.Key, kv => kv.Value.First().Size);

                    var selectedVersion = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select a version:")
                            .PageSize(10)
                            .AddChoices(versions.Keys.Select(v => $"{v} ({versions[v]})"))
                    );

                    string versionKey = selectedVersion.Split(' ')[0];
                    var versionInfo = buildData[versionKey].First();

                    Logger.Info("Enter the directory where you want to save the build:");
                    string downloadDirectory = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(downloadDirectory) || !Directory.Exists(downloadDirectory))
                    {
                        Logger.Error("Invalid directory. Operation aborted.");
                        return false;
                    }

                    string fileName = Path.GetFileName(versionInfo.Url);
                    string cleanFileName = CleanFileName(fileName);
                    string filePath = Path.Combine(downloadDirectory, cleanFileName);

                    try
                    {
                        using var httpClient = new HttpClient();
                        var responseMessage = await httpClient.GetAsync(versionInfo.Url, HttpCompletionOption.ResponseHeadersRead);

                        // Add Fallback links to cdn.xybnetwork.xyz
                        if (!responseMessage.IsSuccessStatusCode)
                        {
                            Logger.Error($"[red]ERROR[/] Download link is not valid");
                            return false;
                        }

                        var totalBytes = responseMessage.Content.Headers.ContentLength ?? -1L;

                        await AnsiConsole.Progress()
                            .HideCompleted(true)
                            .AutoRefresh(true)
                            .Columns(
                                new TaskDescriptionColumn(),  
                                new ProgressBarColumn { CompletedStyle = Style.Parse("green") },
                                new PercentageColumn(),
                                new DownloadedColumn(),
                                new TransferSpeedColumn(),
                                new SpinnerColumn(Spinner.Known.Dots)
    )
                            .StartAsync(async ctx =>
                            {
                                var task = ctx.AddTask($"[green]LOG[/] Downloading {cleanFileName}", maxValue: totalBytes > 0 ? totalBytes : 100);

                                using var responseStream = await responseMessage.Content.ReadAsStreamAsync();
                                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                                byte[] buffer = new byte[262144];
                                int bytesRead;
                                long totalBytesRead = 0;


                                // Set MaxValue only once to prevent flickering
                                if (totalBytes > 0)
                                {
                                    task.MaxValue = totalBytes;
                                }


                                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;

                                    task.Increment(bytesRead);
                                }

                                task.Value = task.MaxValue;  // Ensure full completion

                            });

                        Logger.Info($"Download completed: {filePath}");
                        _downloadCompleted = true; // Set the field to true when download is completed

                        if (cleanFileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            UnzipBuild(filePath, downloadDirectory);
                        }
                        else if (cleanFileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                        {
                            UnrarBuild(filePath, downloadDirectory);
                        }

                        return true;
                    }
                    catch (HttpRequestException httpEx)
                    {
                        Logger.Error($"HTTP Request error: {httpEx.Message}");
                        return false;
                    }
                    catch (Exception downloadEx)
                    {
                        Logger.Error($"Error during file download: {downloadEx.Message}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error in HandleActionAsync: {ex.Message}");
                    return false;
                }
            }
            return false;

        }

        private void UnzipBuild(string filePath, string extractDirectory)
        {
            try
            {
                AnsiConsole.MarkupLine("[yellow]Unpacking the Build...[/]");

                string extractPath = Path.Combine(extractDirectory, Path.GetFileNameWithoutExtension(filePath));
                if (!Directory.Exists(extractPath))
                    Directory.CreateDirectory(extractPath);

                using (var archive = ZipFile.OpenRead(filePath))
                {
                    int total = archive.Entries.Count;
                    int current = 0;

                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(extractPath, entry.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                        if (!string.IsNullOrEmpty(entry.Name))
                            entry.ExtractToFile(destinationPath, true);

                        current++;
                        int percent = (int)(current * 100f / total);
                        AnsiConsole.Markup($"\r[green]Progress:[/] {percent}%   ");
                    }
                }

                AnsiConsole.WriteLine(); // Final newline
                AnsiConsole.MarkupLine($"[green]Build unpacked to:[/] [underline]{extractDirectory}[/]");
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error during unpacking:[/] {ex.Message}");
            }
        }


        private void UnrarBuild(string filePath, string extractDirectory)
        {
            try
            {
                AnsiConsole.MarkupLine("[yellow]Unpacking the build...[/]");

                string extractPath = Path.Combine(extractDirectory, Path.GetFileNameWithoutExtension(filePath));
                if (!Directory.Exists(extractPath))
                    Directory.CreateDirectory(extractPath);

                using var archive = RarArchive.Open(filePath);
                var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
                int total = entries.Count;
                int current = 0;

                foreach (var entry in entries)
                {
                    entry.WriteToDirectory(extractPath, new SharpCompress.Common.ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });

                    current++;
                    int percent = (int)(current * 100f / total);
                    AnsiConsole.Markup($"\r[green]Progress:[/] {percent}%   ");
                }

                AnsiConsole.WriteLine(); // Final newline
                AnsiConsole.MarkupLine($"[green]Build unpacked to:[/] [underline]{extractPath}[/]");
            }
            catch (Exception unrarEx)
            {
                AnsiConsole.MarkupLine($"[red]Error during unpacking:[/] {unrarEx.Message}");
            }
        }


        private string CleanFileName(string fileName)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            string cleanFileName = Regex.Replace(fileName, invalidRegStr, "_");
            cleanFileName = cleanFileName.Trim('.', ' ');

            if (string.IsNullOrWhiteSpace(cleanFileName))
            {
                cleanFileName = "unnamed_file";
            }

            int maxFileNameLength = 255;
            if (cleanFileName.Length > maxFileNameLength)
            {
                string extension = Path.GetExtension(cleanFileName);
                cleanFileName = cleanFileName.Substring(0, maxFileNameLength - extension.Length) + extension;
            }

            return cleanFileName;
        }
    }

    public class BuildInfo
    {
        public string Url { get; set; }
        public string Movie { get; set; }
        public string Size { get; set; }
        public string BackgroundImage { get; set; }
        public string SeasonNumber { get; set; }
    }
}