using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static XybLauncher.VersionHandler;

namespace XybLauncher
{


    // hold all functions for passing the arguments here and just call them in client manager.cs
    public class CustomArguments
    {
        
        public readonly string _otarguments = "-epicportal -epiclocale=en-us -skippatchcheck -HTTP=WinInet -NOSSLPINNING -AUTH_PASSWORD=5001 -AUTH_LOGIN=unknown -AUTH_TYPE=exchangecode";

        private readonly string default_arguments = @"
        -NOSSLPINNING 
        -epicapp=Fortnite 
        -epicenv=Prod 
        -epiclocale=en-us 
        -epicportal 
        -skippatchcheck 
        -nobe 
        -fromfl=eac 
        -fltoken=3db3ba5dcbd2e16703f3978d 
        -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ 
        -AUTH_LOGIN={email} 
        -AUTH_PASSWORD={password} 
        -AUTH_TYPE=epic";

        public string GetDefaultArgs()
        {
            var json = File.ReadAllText(versionsdata);
            var fortniteVersions = JsonConvert.DeserializeObject<FortniteVersions>(json);

            var selectedVersion = fortniteVersions?.Versions.Find(v => v.Season == fortniteVersions.Selected);


            return selectedVersion?.IsOnlineTest == true ? _otarguments : default_arguments;
        }



        private static string versionsdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/versions.json";

        public static string GetBuildArchitecture()
        {
            var json = File.ReadAllText(versionsdata);
            var fortniteVersions = JsonConvert.DeserializeObject<FortniteVersions>(json);


            var selectedVersion = fortniteVersions?.Versions.Find(v => v.Season == fortniteVersions.Selected);

            return selectedVersion?.IsOnlineTest == true ? "Win32" : "Win64";
        }







    }
}
