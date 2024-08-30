using System.Diagnostics;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Spectre.Console;

namespace MomentumLauncher;
public class Utilities
{

    public static void DownloadFile(string Url, string Path) => new WebClient().DownloadFile(Url, Path);

    public static async void StartGame(string gamePath, string dllDownload, string dllName)
    {
        AnsiConsole.MarkupLine("[blue]Starting Fortnite...[/]");

        string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher";
        string outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.txt");

        if (!File.Exists(gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe"))
            DownloadFile("https://www.dropbox.com/scl/fi/g9zq6w1xauufioes2zb4j/FortniteLauncher.exe?rlkey=7u5ib9b0swplxiz3phg6wcae6&st=ct93lwhm&dl=1", gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe");
        if (!File.Exists(appdata + "\\" + dllName))
            DownloadFile(dllDownload, appdata + "\\" + dllName);

        string emailPath = Path.Combine(appdata, "email.txt");
        string passwordPath = Path.Combine(appdata, "password.txt");

        string email = File.ReadAllText(emailPath);
        string password = File.ReadAllText(passwordPath);

        string launcherexePath = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncherPatch.exe";
        Process launcher = new Process();
        launcher.StartInfo.FileName = launcherexePath;
        launcher.StartInfo.WorkingDirectory = Path.GetDirectoryName(launcherexePath);
        launcher.StartInfo.Arguments = $" -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -nosplash -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={email} -AUTH_PASSWORD={password} -AUTH_TYPE=epic";
    

        Process shippingbe = new Process();
        shippingbe.StartInfo.FileName = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe";

        Process shipping = new Process();
        shipping.StartInfo.FileName = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe";
        shipping.StartInfo.Arguments = $" -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -nosplash -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={email} -AUTH_PASSWORD={password} -AUTH_TYPE=epic";
        shipping.StartInfo.UseShellExecute = false;
        shipping.StartInfo.RedirectStandardOutput = true;
        shipping.StartInfo.RedirectStandardError = true;

      //  launcher.Start();
      //  await Task.Delay(5000); // Wait for the launcher to start the game

       // Process[] processes = Process.GetProcessesByName("FortniteClient-Win64-Shipping");
      //  if (processes.Length > 0)
      //  {
          //  MomentumLauncher.Injector.Inject(processes[0].Id, appdata + "\\" + dllName);
       // }



        shipping.Start();
        MomentumLauncher.Injector.Inject(shipping.Id, appdata + "\\" + dllName);



        Environment.Exit(0);
    }






    //Server Start Module
    public static async void StartServer(string gamePath, string dllDownload, string dllName)
    {

        string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher";

        if (!File.Exists(gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe"))
            DownloadFile("https://www.dropbox.com/scl/fi/g9zq6w1xauufioes2zb4j/FortniteLauncher.exe?rlkey=7u5ib9b0swplxiz3phg6wcae6&st=ct93lwhm&dl=1", gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe");
        if (!File.Exists(appdata + "\\" + dllName))
            DownloadFile(dllDownload, appdata + "\\" + dllName);

        if (!File.Exists(gamePath + "\\" + "reboot.dll"))
            DownloadFile("https://www.dropbox.com/scl/fi/dmxh4bapxplza1b49qjfh/RebootModified.dll?rlkey=4mhse96gmunvzkorn4wrsm41o&st=o43za5ue&dl=1", appdata + "//" + "reboot.dll");


        string serveremailPath = Path.Combine(appdata, "serveremail.txt");
        string serverpasswordPath = Path.Combine(appdata, "serverpassword.txt");

        string serveremail = File.ReadAllText(serveremailPath);
        string serverpassword = File.ReadAllText(serverpasswordPath);
        string launcherexePath = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe";

        Process launcher = new Process();
        launcher.StartInfo.FileName = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe";
        launcher.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(launcherexePath);
        launcher.StartInfo.Arguments = $" -log -nullhri -nosound -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=none -nosplash -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={serveremail} -AUTH_PASSWORD={serverpassword} -AUTH_TYPE=epic";


        Process servershipping = new Process();
        servershipping.StartInfo.FileName = gamePath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe";
        servershipping.StartInfo.Arguments =
            $"-log -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -nosplash -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_LOGIN={serveremail} -AUTH_PASSWORD={serverpassword} -AUTH_TYPE=epic -nullrhi -nosound";
        servershipping.StartInfo.UseShellExecute = false;
        servershipping.StartInfo.RedirectStandardOutput = false;
        servershipping.StartInfo.RedirectStandardError = true;


    //servershipping.Start();
    //MomentumLauncher.Injector.Inject(servershipping.Id, appdata + "\\" + dllName);
       servershipping.Start();
        await Task.Delay(5000); // Wait for the launcher to start the game

        Process[] processes = Process.GetProcessesByName("FortniteClient-Win64-Shipping");
        if (processes.Length > 0)
        {
            MomentumLauncher.Injector.Inject(processes[0].Id, appdata + "\\" + dllName);
        }




        Environment.Exit(0);


    }

}