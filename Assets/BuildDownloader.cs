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
                                new TaskDescriptionColumn(),  // Task description
                                new ProgressBarColumn { CompletedStyle = Style.Parse("green") },  // Green progress bar
                                new PercentageColumn(),  // Show percentage
                                new DownloadedColumn(),         // Downloaded
                                new TransferSpeedColumn(),     // Transfer speed
                                new SpinnerColumn(Spinner.Known.Dots)  // Spinner for visual feedback
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
                            UnzipFile(filePath, downloadDirectory);
                        }
                        else if (cleanFileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                        {
                            UnrarFile(filePath, downloadDirectory);
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

        private void UnzipFile(string filePath, string extractDirectory)
        {
            try
            {
                Logger.Info("Unzipping the file...");

                string extractPath = Path.Combine(extractDirectory, Path.GetFileNameWithoutExtension(filePath));

                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                ZipFile.ExtractToDirectory(filePath, extractPath);
                Logger.Info($"File unzipped to: {extractPath}");
                File.Delete(filePath);
            }
            catch (Exception unzipEx)
            {
                Logger.Error($"Error during unzipping: {unzipEx.Message}");
            }
        }

        private void UnrarFile(string filePath, string extractDirectory)
        {
            try
            {
                Logger.Info("Unpacking the build...");

                string extractPath = Path.Combine(extractDirectory, Path.GetFileNameWithoutExtension(filePath));

                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                using var archive = RarArchive.Open(filePath);
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(extractPath, new SharpCompress.Common.ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
                Logger.Info($"Build unpacked to: {extractPath}");
            }
            catch (Exception unrarEx)
            {
                Logger.Error($"Error during unpacking: {unrarEx.Message}");
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