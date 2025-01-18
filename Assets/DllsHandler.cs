using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XybLauncher;

public class DllsHandler
{

    public static void DownloadFile(string Url, string Path) => new WebClient().DownloadFile(Url, Path);

    public static void DownloadFiles()
    {


        // Make all dlls download in a subfolder 


        string appfolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher";

        if (!File.Exists(appfolder + "\\Cobalt.dll"))
            DownloadFile("https://www.dropbox.com/scl/fi/m996mhjy77qn2t3bfxq6l/Cobalt.dll?rlkey=6araxm5ngyznp4fmvxqtgm9a7&st=1o2rxx0l&dl=1", appfolder + "\\Cobalt.dll");


    }


}
