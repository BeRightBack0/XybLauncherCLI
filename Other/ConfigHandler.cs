using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Spectre.Console;

namespace XybLauncher.Other
{
    public class ConfigHandler
    {





        // Create config.json rewrite it pls 
        private static string configfile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XybLauncher/config.json";

        public static void ChkConfig()
        {
            if (!File.Exists(configfile))
            {

                Directory.CreateDirectory(Path.GetDirectoryName(configfile)!);

                
                var defaultConfig = new
                {
                    Name = "XybLauncher",
                    Version = "1.0",
                    CreatedDate = DateTime.UtcNow
                };

                File.WriteAllText(configfile, JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                return;
            }
        }




    }
}
