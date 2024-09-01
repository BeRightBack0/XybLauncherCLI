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

namespace XybLauncher
{
    public class FilesManager
    {


       


        private readonly HttpClient _httpClient;

        public FilesManager()
        {
            _httpClient = new HttpClient();
        }


        public async Task HandleActionAsync(string action)
        {
            if (action == "DownloadBuild")
            {
                try
                {

                    // Make the API call and get the response as a string
                    string apiUrl = "https://xyb-launcher-api.vercel.app/builds";
                    using HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var buildData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonResponse);
                    var versions = buildData?.Keys.ToList() ?? new List<string>();

                    if (versions.Count == 0)
                    {
                        Console.WriteLine("No valid choices were returned from the API.");
                        return;
                    }

                    var selectedVersion = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select a version:")
                            .PageSize(10)
                            .AddChoices(versions)
                    );

                    var url = buildData[selectedVersion].First();

                    Console.WriteLine("Enter the directory where you want to save the build:");
                    string downloadDirectory = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(downloadDirectory) || !Directory.Exists(downloadDirectory))
                    {
                        Console.WriteLine("Invalid directory. Operation aborted.");
                        return;
                    }

                    string fileName = Path.GetFileName(url);
                    string cleanFileName = CleanFileName(fileName);
                    string filePath = Path.Combine(downloadDirectory, cleanFileName);

                    try
                    {
                        using var httpClient = new HttpClient();
                        var responseMessage = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                        if (!responseMessage.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"HTTP Request error: {responseMessage.ReasonPhrase}");
                            return;
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

                        // Check the file extension to determine whether to unzip or unrar
                        if (cleanFileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            UnzipFile(filePath, downloadDirectory);
                        }
                        else if (cleanFileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                        {
                            UnrarFile(filePath, downloadDirectory);
                        }
                    }
                    catch (HttpRequestException httpEx)
                    {
                        Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                    }
                    catch (Exception downloadEx)
                    {
                        Console.WriteLine($"Error during file download: {downloadEx.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in HandleActionAsync: {ex.Message}");
                }
            }
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
            // Remove invalid characters
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            // Replace invalid characters with underscores
            string cleanFileName = Regex.Replace(fileName, invalidRegStr, "_");

            // Trim leading/trailing periods and spaces
            cleanFileName = cleanFileName.Trim('.', ' ');

            // Ensure the file name is not empty
            if (string.IsNullOrWhiteSpace(cleanFileName))
            {
                cleanFileName = "unnamed_file";
            }

            // Truncate if the file name is too long (Windows has a 260 character path limit)
            int maxFileNameLength = 255; // Maximum file name length
            if (cleanFileName.Length > maxFileNameLength)
            {
                string extension = Path.GetExtension(cleanFileName);
                cleanFileName = cleanFileName.Substring(0, maxFileNameLength - extension.Length) + extension;
            }

            return cleanFileName;
        }









        private async Task DownloadBuildAsync(string apiUrl)
        {
            try
            {
                Console.WriteLine("Starting download...");

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();

                ChoiceContainer choices = JsonConvert.DeserializeObject<ChoiceContainer>(jsonString);
                if (choices?.Choices == null || choices.Choices.Count == 0)
                {
                    Console.WriteLine("No valid choices were returned from the API.");
                    return;
                }

                ProcessChoices(choices);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network error occurred: " + ex.Message);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON deserialization error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);
            }
        }

        private void ProcessChoices(ChoiceContainer choices)
        {
            foreach (var choice in choices.Choices)
            {
                Console.WriteLine($"Choice: {choice.Name}");
                ExecuteAction(choice.Action);
            }
        }

        private void ExecuteAction(string action)
        {
            switch (action)
            {
                case "Action1":
                    Console.WriteLine("Performing Action1...");
                    break;
                case "Action2":
                    Console.WriteLine("Performing Action2...");
                    break;
                default:
                    Console.WriteLine("Unknown action");
                    break;
            }
        }
    }

    public class Choice
    {
        public string Name { get; set; }
        public string Action { get; set; }
    }

    public class ChoiceContainer
    {
        public List<Choice> Choices { get; set; }
    }



}
