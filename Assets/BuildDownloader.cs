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
using static XybLauncher.Assets.Errors;

namespace XybLauncher
{
    public class BuildDownloader
    {
        private readonly HttpClient _httpClient;
        private bool _downloadCompleted = false;

        public BuildDownloader()
        {
            _httpClient = new HttpClient();
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
                        Console.WriteLine("API is down");
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

                    Console.WriteLine("Enter the directory where you want to save the build:");
                    string downloadDirectory = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(downloadDirectory) || !Directory.Exists(downloadDirectory))
                    {
                        Console.WriteLine("Invalid directory. Operation aborted.");
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
                            Console.WriteLine($"[red]ERROR[/] Download link is not valid");
                            return false;
                        }

                        var totalBytes = responseMessage.Content.Headers.ContentLength ?? -1L;

                        await AnsiConsole.Progress()
                            .StartAsync(async ctx =>
                            {
                                var task = ctx.AddTask($"[green]LOG[/] Downloading {cleanFileName}", maxValue: totalBytes > 0 ? totalBytes : 100);

                                using var responseStream = await responseMessage.Content.ReadAsStreamAsync();
                                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                                byte[] buffer = new byte[262144];
                                int bytesRead;
                                long totalBytesRead = 0;

                                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;

                                    task.Increment(bytesRead);
                                    if (totalBytes > 0)
                                    {
                                        task.MaxValue = totalBytes;
                                    }
                                }
                            });

                        Console.WriteLine($"Download completed: {filePath}");

                        if (cleanFileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            UnzipFile(filePath, downloadDirectory);
                        }
                        else if (cleanFileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                        {
                            UnrarFile(filePath, downloadDirectory);
                        }

                        _downloadCompleted = true;
                        return true;
                    }
                    catch (HttpRequestException httpEx)
                    {
                        Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                        return false;
                    }
                    catch (Exception downloadEx)
                    {
                        Console.WriteLine($"Error during file download: {downloadEx.Message}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in HandleActionAsync: {ex.Message}");
                    return false;
                }
            }
            return false;
        }

        private void UnzipFile(string filePath, string extractDirectory)
        {
            try
            {
                Console.WriteLine("Unzipping the file...");

                string extractPath = Path.Combine(extractDirectory, Path.GetFileNameWithoutExtension(filePath));

                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                ZipFile.ExtractToDirectory(filePath, extractPath);
                Console.WriteLine($"File unzipped to: {extractPath}");
                File.Delete(filePath);
            }
            catch (Exception unzipEx)
            {
                Console.WriteLine($"Error during unzipping: {unzipEx.Message}");
            }
        }

        private void UnrarFile(string filePath, string extractDirectory)
        {
            try
            {
                Console.WriteLine("Unpacking the build...");

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
                Console.WriteLine($"Build unpacked to: {extractPath}");
            }
            catch (Exception unrarEx)
            {
                Console.WriteLine($"Error during unpacking: {unrarEx.Message}");
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