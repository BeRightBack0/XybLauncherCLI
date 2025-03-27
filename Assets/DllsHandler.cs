using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Spectre.Console;
using XybLauncher.Other;

namespace XybLauncher;

public class DllsHandler
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly HttpClient httpClient = new HttpClient();

    private static async Task DownloadFileAsync(string url, string path)
    {
        using (var response = await httpClient.GetAsync(url))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(path, content);
        }
    }

    public static async Task DownloadRedirectsAsync()
    {
        string appfolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher\\Libraries\\Redirects";

        if (!Directory.Exists(appfolder))
        {
            Logger.Debug($"Creating {appfolder}");
            Directory.CreateDirectory(appfolder);
        }

          if (!File.Exists(appfolder + "\\CobaltLocal.dll"))
          await DownloadFileAsync("link", appfolder + "\\CobaltLocal.dll");

          if (!File.Exists(appfolder + "\\CobaltXybNetwork.dll"))
          await DownloadFileAsync("link", appfolder + "\\CobaltXybNetwork.dll");
    }

    public static async Task DownloadGameServersAsync()
    {
        string gsfolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher\\Libraries\\GameServers";

        if (!Directory.Exists(gsfolder))
        {
            Logger.Debug($"Creating {gsfolder}");
            Directory.CreateDirectory(gsfolder);
        }

        if (!File.Exists(gsfolder + "\\Volcano-8.51.dll"))
            await DownloadFileAsync("https://www.dropbox.com/scl/fi/tkccozmil5y2e1omqgcd3/Volcano-8.51.dll?rlkey=ldupdjrwefnrno98tm538lt9y&st=bqg5nw7k&dl=1", gsfolder + "\\Volcano-8.51.dll");

        if (!File.Exists(gsfolder + "\\JGS-2.4.22.dll"))
            await DownloadFileAsync("https://www.dropbox.com/scl/fi/h7jw3mx17awjnlncp5vnn/JGS2.4.22.dll?rlkey=na8ystzwxe3eeayyp336qubvo&st=6uloixyj&dl=1", gsfolder + "\\JGS-2.4.22.dll");

        if (!File.Exists(gsfolder + "\\Reboot-3.0.dll"))
            await DownloadFileAsync("https://www.dropbox.com/scl/fi/ioh9sj74c8d4pitn441e8/Reboot-3.0.dll?rlkey=rt3ridf872x11k8jtdpxhk31n&st=54iaeq96&dl=1", gsfolder + "\\Reboot-3.0.dll");
    }

    public static async Task DownloadDefaultRedirectsAsync()
    {
        string redirectsfolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher\\Libraries\\Redirects";

        if (!Directory.Exists(redirectsfolder))
        {
            Logger.Debug($"Creating {redirectsfolder}");
            Directory.CreateDirectory(redirectsfolder);
        }

        if (!File.Exists(redirectsfolder + "\\CobaltLocal.dll"))
            await DownloadFileAsync("https://www.dropbox.com/scl/fi/tu8f3kqexwsyll03q254t/CobaltLocal.dll?rlkey=cyob9pyd59knmawylob8s8fff&st=y55vtoyn&dl=1", redirectsfolder + "\\CobaltLocal.dll");

        if (!File.Exists(redirectsfolder + "\\SinumLocal.dll"))
            await DownloadFileAsync("https://www.dropbox.com/scl/fi/hsopu9rp1hlq0uj4fe2zj/SinumLocal.dll?rlkey=kysuileif7j6eujcvaknh8xi8&st=k7z8l9i7&dl=1", redirectsfolder + "\\SinumLocal.dll");
    }

    public static void ShowSelectedLibraries()
    {
        string clientRedirect = ConfigHandler.GetSettingValue("RedirectSettings", "ClientRedirect");
        string serverRedirect = ConfigHandler.GetSettingValue("RedirectSettings", "ServerRedirect");
        AnsiConsole.MarkupLine($"Selected Client Redirect: [blue]{clientRedirect}[/]");
        AnsiConsole.MarkupLine($"Selected Server Redirect: [blue]{serverRedirect}[/]");
    }
}
